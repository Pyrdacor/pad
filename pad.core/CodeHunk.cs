using pad.core.interfaces;
using pad.core.opcodes;

namespace pad.core
{
    public class CodeHunk
    {
        public record Reference
        (
            string Label,
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
        readonly Dictionary<int, string> labels = new();
        readonly List<UnresolvedJump> unresolvedJumps = new();
        string asm = "";

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
        public string Asm => asm;
        public IReadOnlyList<Reference>? this[int index] => references.TryGetValue(index, out var refs) ? refs : null;
        public IReadOnlyList<UnresolvedJump> UnresolvedJumps => unresolvedJumps;

        public void Process(List<uint> hunkOffsets, int offset = 0, string newline = "\n")
        {
            asm = "";
            references.Clear();
            dataReader.Position = offset;
            HashSet<int> processedOffsets = new();
            HashSet<int> branchOffsets = new();
            string currentAsm = "";
            Dictionary<string, string> currentAsmLabelReplacements = new();
            var handlers = new OpcodeHandlers(AsmOutputHandler, BranchHandler, JumpHandler, ReferenceHandler);            

            while (dataReader.Position < dataReader.Size)
            {
                if (processedOffsets.Contains(dataReader.Position))
                    break; // all done

                OpcodeProcessor.Process(dataReader, handlers);

                asm += ReplaceLabels(currentAsm) + newline;
                currentAsmLabelReplacements.Clear();
            }

            string ReplaceLabels(string asm)
            {
                foreach (var replacement in currentAsmLabelReplacements)
                    asm = asm.Replace(replacement.Key, replacement.Value);

                return asm;
            }

            void AsmOutputHandler(string asm)
            {
                currentAsm = asm;
            }

            void BranchHandler(uint targetLocation, bool unconditional)
            {
                if (unconditional)
                    dataReader.Position = (int)targetLocation;
                else
                    branchOffsets.Add((int)targetLocation);
            }

            void JumpHandler(uint jumpCallOffset, string jumpTarget)
            {
                if (jumpTarget.StartsWith(Global.LabelPrefix))
                {
                    if (!relocs.TryGetValue(jumpCallOffset, out int hunkIndex))
                        throw new InvalidDataException($"Missing reloc entry for jump instruction in hunk {Index} at offset ${jumpCallOffset-2:x8}.");
                }
            }

            void ReferenceHandler(Dictionary<string, uint> references)
            {
                foreach (var reference in references)
                {
                    if (!relocs.TryGetValue(reference.Value, out int hunkIndex))
                        throw new InvalidDataException($"Missing reloc entry for reference in hunk {Index} at offset ${reference.Value:x8}.");

                    string label = reference.Key;
                    string replacement = GetReplacement(label, hunkOffsets[hunkIndex], out var relativeOffset);

                    currentAsmLabelReplacements.Add(label, replacement);
                    this.references[hunkIndex].Add(new Reference(replacement, relativeOffset));
                }

                static string GetReplacement(string label, uint hunkOffset, out uint relativeOffset)
                {
                    relativeOffset = uint.Parse(label[4..12], System.Globalization.NumberStyles.AllowHexSpecifier);
                    return label[0..4] + $"{hunkOffset + relativeOffset:x8}";
                }
            }
        }
    }
}
