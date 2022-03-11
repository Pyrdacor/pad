using pad.core.interfaces;
using pad.core.opcodes;

namespace pad.core
{
    public class CodeHunk
    {
        public record Reference
        (
            string Label,
            int SourceHunkIndex,
            uint SourceOffset,            
            int TargetHunkIndex,
            uint RelativeOffset
        );

        public record UnresolvedJump
        (
            string Target,
            uint JumpCallOffset
        );

        readonly IDataReader dataReader;
        // Key = Usage offset inside this code hunk, Value = Target hunk index
        readonly Dictionary<uint, int> relocs;
        readonly Dictionary<int, List<Reference>> references = new();
        // Key = Offset inside this code hunk, Value = Label name
        readonly Dictionary<uint, string> labels = new();
        // Jumps which use variables for the target address
        readonly List<UnresolvedJump> unresolvedJumps = new();
        // Jumps to other code hunks
        readonly Dictionary<int, List<UnresolvedJump>> externalJumps = new();
        readonly Dictionary<int, string> asm = new();

        public CodeHunk(int index, IDataReader dataReader, Dictionary<uint, List<uint>> relocs)
        {
            Index = index;

            foreach (var hunkIndex in relocs.Keys)
                references.Add((int)hunkIndex, new());

            this.dataReader = dataReader;
            this.relocs = new();

            foreach (var reloc in relocs)
            {
                int targetHunk = (int)reloc.Key;

                foreach (var offset in reloc.Value)
                    this.relocs.Add(offset, targetHunk);
            }
        }

        public int Index { get; }
        public string Asm(string newline = "\n") => string.Join(newline, asm);
        public IReadOnlyList<Reference>? this[int index] => references.TryGetValue(index, out var refs) ? refs : null;
        public IReadOnlyList<UnresolvedJump> UnresolvedJumps => unresolvedJumps;

        public void Process(List<uint> hunkOffsets, int offset = 0)
        {
            asm.Clear();
            foreach (var reference in references)
                reference.Value.Clear();
            dataReader.Position = offset;
            HashSet<int> processedOffsets = new();
            HashSet<int> branchOffsets = new();
            HashSet<int> processedBranchOffsets = new();
            string currentAsm = "";
            Dictionary<string, string> currentAsmLabelReplacements = new();
            int codeOffset = 0;
            bool stop = false;
            var handlers = new OpcodeHandlers(AsmOutputHandler, BranchHandler, JumpHandler, ReferenceHandler, OpcodeSizeHandler);           

        Process:
            while (dataReader.Position < dataReader.Size)
            {
                codeOffset = dataReader.Position;

                if (processedOffsets.Contains(dataReader.Position))
                    break; // all done

                // TODO: also mark additional words of that instruction as "processed"
                // The total length is given in the opcode, just skip the first two bytes.
                // The size is always a multiple of 2.

                processedOffsets.Add(codeOffset);
                stop = false;

                OpcodeProcessor.ProcessNextOpcode(dataReader, handlers);

                stop = stop || currentAsm.StartsWith("RT") || currentAsm.StartsWith("TRAP");

                asm.Add(codeOffset, ReplaceLabels(currentAsm));
                currentAsmLabelReplacements.Clear();

                if (stop)
                    break;
            }

            if (branchOffsets.Count != 0)
            {
                dataReader.Position = branchOffsets.Last();
                branchOffsets.Remove(dataReader.Position);
                processedBranchOffsets.Add(dataReader.Position);
                goto Process;
            }

            string ReplaceLabels(string asm)
            {
                foreach (var replacement in currentAsmLabelReplacements)
                    asm = asm.Replace(replacement.Key, replacement.Value);

                return asm;
            }

            void OpcodeSizeHandler(int size)
            {
                if (size <= 2)
                    return;

                int numWords = (size - 2) / 2;

                for (int i = 1; i <= numWords; ++i)
                    processedOffsets.Add(codeOffset + i * 2);
            }

            void AsmOutputHandler(string asm)
            {
                currentAsm = asm;
            }

            void BranchHandler(uint targetLocation, bool unconditional)
            {
                int location = (int)targetLocation;

                if (unconditional)
                    dataReader.Position = location;
                else if (!processedBranchOffsets.Contains(location))
                    branchOffsets.Add(location);
            }

            void JumpHandler(uint jumpCallOffset, string jumpTarget, bool subRoutine)
            {
                if (jumpTarget.StartsWith(Global.LabelPrefix))
                {
                    if (!relocs.TryGetValue(jumpCallOffset, out int hunkIndex))
                        throw new InvalidDataException($"Missing reloc entry for jump instruction in hunk {Index} at offset ${jumpCallOffset-2:x8}.");

                    if (hunkIndex != Index)
                    {
                        if (!externalJumps.ContainsKey(hunkIndex))
                            externalJumps[hunkIndex] = new() { new UnresolvedJump(jumpTarget, jumpCallOffset) };
                        else
                            externalJumps[hunkIndex].Add(new UnresolvedJump(jumpTarget, jumpCallOffset));

                        // A jump to a sub-routine will always return at some time or at least is expected to, so continue after the instruction in this case.
                        // On the other hand a normal jump can not be expected to return, so just stop execution here.
                        if (!subRoutine)
                            stop = true;
                    }
                    else
                    {
                        if (subRoutine) // Sub-routines will return, so the code after this instruction will be called as well. Mark it as a branch to process it later.
                            branchOffsets.Add(dataReader.Position);
                        dataReader.Position = int.Parse(jumpTarget[4..12], System.Globalization.NumberStyles.AllowHexSpecifier);
                    }
                }
                else
                {
                    unresolvedJumps.Add(new UnresolvedJump(jumpTarget, jumpCallOffset));

                    // A jump to a sub-routine will always return at some time or at least is expected to, so continue after the instruction in this case.
                    // On the other hand a normal jump can not be expected to return, so just stop execution here.
                    if (!subRoutine) 
                        stop = true;
                }
            }

            void ReferenceHandler(Dictionary<string, opcodes.Reference> references)
            {
                foreach (var reference in references)
                {
                    if (!relocs.TryGetValue(reference.Value.UsageOffset, out int hunkIndex))
                    {
                        if (reference.Value.RelativeAddress == 0x00000004 || // special address
                            reference.Value.RelativeAddress >= 0x00dff000 && reference.Value.RelativeAddress < 0x00e00000) // Amiga hardware registers
                        {
                            currentAsmLabelReplacements.Add(reference.Key, $"#${reference.Value.RelativeAddress:x8}");
                            continue;
                        }

                        throw new InvalidDataException($"Missing reloc entry for reference in hunk {Index} at offset ${reference.Value:x8}.");
                    }

                    string label = reference.Key;
                    string replacement = GetReplacement(label, hunkOffsets[hunkIndex], reference.Value.RelativeAddress);

                    currentAsmLabelReplacements.Add(label, replacement);
                    this.references[hunkIndex].Add(new Reference(replacement, Index, reference.Value.UsageOffset, hunkIndex, reference.Value.RelativeAddress));
                    if (hunkIndex == Index)
                        labels.TryAdd(reference.Value.RelativeAddress, replacement);
                }

                static string GetReplacement(string label, uint hunkOffset, uint relativeOffset)
                {
                    return label[0..4] + $"{hunkOffset + relativeOffset:x8}";
                }
            }
        }
    }
}
