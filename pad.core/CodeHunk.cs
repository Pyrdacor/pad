using pad.core.interfaces;
using pad.core.opcodes;
using pad.core.serialization;
using System.Text.RegularExpressions;

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
        readonly HashSet<int> processedOffsets = new();
        readonly string[] addressRegisters = new string[7]; // A0 to A6 only
        readonly Regex moveaRegex = new Regex(@"MOVEA\.[WL] (.*),A([0-6])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex leaRegex = new Regex(@"LEA\.[WL] (.*),A([0-6])", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
        public string Asm(string newline = "\n")
        {
            var asm = new SortedDictionary<int, string>(this.asm);

            // Add labels
            foreach (var label in labels)
            {
                string labelName = label.Value;

                if (labelName.StartsWith(Global.DataPrefix) && asm.ContainsKey((int)label.Key))
                {
                    // This means there is a code entry at that label but we treated this as data
                    // instead of a code label. This may happen when using instructions like LEA
                    // to store a code pointer first and jump to it later.
                    labelName = Global.LabelPrefix + labelName[4..];
                }
                asm.TryAdd((int)label.Key - 1, newline + labelName + ":");
            }

            // Fill in data
            int wordIndex = 1;
            int stringIndex = 1;
            List<byte> currentData = new();
            int dataOffset = 0;
            for (int i = 0; i < dataReader.Size / 2; ++i)
            {
                int offset = i * 2;

                if (!processedOffsets.Contains(offset))
                {
                    if (currentData.Count == 0)
                        dataOffset = offset;
                    else if (labels.ContainsKey((uint)offset))
                    {
                        OutputData();
                        currentData.Clear();
                        dataOffset = offset;
                    }
                    dataReader.Position = offset;
                    currentData.Add(dataReader.ReadByte());
                    currentData.Add(dataReader.ReadByte());
                }
                else if (currentData.Count != 0)
                {
                    OutputData();
                    currentData.Clear();
                }
            }
            if (currentData.Count != 0)
                OutputData();

            void OutputData()
            {
                // Most likely a string
                if (currentData[^1] == 0x00 && !currentData.Any(b => b != 0x00 && (b < 0x20 || b > 0x7e)) && currentData.Any(b => b != 0x00))
                {
                    string dataString = "";
                    List<byte> stringDataBuffer = new();

                    foreach (var data in currentData)
                    {
                        if (data == 0x00)
                        {
                            dataString += $"Str{stringIndex++:x6}\t\tdc.b \"" + AmigaExecutable.Encoding.GetString(stringDataBuffer.ToArray()) + "\",0" + newline;
                            stringDataBuffer.Clear();
                        }
                        else
                        {
                            stringDataBuffer.Add(data);
                        }
                    }

                    if (stringDataBuffer.Count != 0)
                        dataString += $"Str{stringIndex++:x6}\t\tdc.b \"" + AmigaExecutable.Encoding.GetString(stringDataBuffer.ToArray()) + "\",0" + newline;

                    asm.Add(dataOffset, newline + dataString.TrimEnd());
                }
                else
                {
                    for (int i = 0; i < currentData.Count / 2; ++i)
                    {
                        string prefix = i == 0 ? newline : "";
                        asm.Add(dataOffset + i * 2, prefix + $"Dat{wordIndex++:x6}\t\tdc.w ${((uint)currentData[i * 2] << 8) | currentData[i * 2 + 1]:x4}");
                    }
                }
            }

            return string.Join(newline, asm.Values);
        }
        public IReadOnlyList<Reference>? this[int index] => references.TryGetValue(index, out var refs) ? refs : null;
        public IReadOnlyList<UnresolvedJump> UnresolvedJumps => unresolvedJumps;

        public void Process(List<uint> hunkOffsets, int offset = 0)
        {
            processedOffsets.Clear();
            asm.Clear();
            foreach (var reference in references)
                reference.Value.Clear();
            dataReader.Position = offset;
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

                processedOffsets.Add(codeOffset);
                stop = false;

                OpcodeProcessor.ProcessNextOpcode(dataReader, handlers);

                stop = stop || currentAsm.StartsWith("RT") || currentAsm.StartsWith("TRAP");
                currentAsm = ReplaceLabels(currentAsm);

                void CheckAddressRegister(Regex regex)
                {
                    var match = regex.Match(currentAsm);

                    if (match.Success)
                        addressRegisters[int.Parse(match.Groups[2].Value)] = match.Groups[1].Value;
                }

                CheckAddressRegister(leaRegex);
                CheckAddressRegister(moveaRegex);

                asm.Add(codeOffset, "\t\t\t\t" + currentAsm);
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
                string label = string.Format(Global.LabelFormatString, location);

                currentAsmLabelReplacements.Add("<LABEL>", label);
                labels.TryAdd(targetLocation, label);

                if (unconditional)
                    dataReader.Position = location;
                else if (!processedBranchOffsets.Contains(location))
                    branchOffsets.Add(location);
            }

            void JumpHandler(uint jumpCallOffset, string jumpTarget, bool subRoutine)
            {
                if (jumpTarget.StartsWith(Global.LabelPrefix) || jumpTarget.StartsWith(Global.FunctionPrefix))
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
                        // Sub-routines will return, so the code after this instruction will be called as well. Mark it as a branch to process it later.
                        if (subRoutine && !processedBranchOffsets.Contains(dataReader.Position))
                            branchOffsets.Add(dataReader.Position);
                        dataReader.Position = int.Parse(jumpTarget[4..12], System.Globalization.NumberStyles.AllowHexSpecifier);
                    }
                }
                else
                {
                    bool MatchDisplacement(string pattern, out string displacement, out int index)
                    {
                        var match = Regex.Match(jumpTarget, pattern);

                        if (match.Success)
                        {
                            displacement = match.Groups[1].Value;
                            index = int.Parse(match.Groups[2].Value);
                            return true;
                        }

                        displacement = "";
                        index = 0;
                        return false;
                    }

                    bool Match(string pattern, out int index)
                    {
                        var match = Regex.Match(jumpTarget, pattern);

                        if (match.Success)
                        {
                            index = int.Parse(match.Groups[1].Value);
                            return true;
                        }

                        index = 0;
                        return false;
                    }

                    int index;

                    if (Match(@"\(A([0-6])\)", out index))
                    {
                        if (HandleAddress(index))
                            return;
                    }
                    else if (Match(@"-\(A([0-6])\)", out index))
                    {
                        if (HandleAddress(index))
                            return;
                    }
                    else if (Match(@"\(A([0-6])\)+", out index))
                    {
                        if (HandleAddress(index))
                            return;
                    }
                    else if (MatchDisplacement(@"\((.*),A([0-6])\)", out string displacement, out index))
                    {
                        // TODO

                        if (HandleAddress(index))
                            return;
                    }
                    else
                    {
                        // PC + displacement
                        var match = Regex.Match(jumpTarget, @"^\(\$([0-9a-fA-F]+),PC\)$");

                        if (match.Success)
                        {
                            int d = int.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.AllowHexSpecifier);
                            int address = (int)jumpCallOffset + d;

                            if (address >= 0 && address < dataReader.Size)
                            {
                                labels.Add((uint)address, string.Format(subRoutine ? Global.FunctionFormatString : Global.LabelFormatString, hunkOffsets[Index] + address));
                                if (subRoutine && !processedBranchOffsets.Contains(dataReader.Position))
                                    branchOffsets.Add(dataReader.Position);
                                dataReader.Position = address;
                                return;
                            }
                            else if (address >= dataReader.Size)
                            {
                                throw new IndexOutOfRangeException("Jump address was outside of range.");
                            }
                        }

                        // TODO: PC + index
                    }

                    bool HandleAddress(int index)
                    {
                        if (addressRegisters[index] is not null)
                        {
                            if (addressRegisters[index].StartsWith(Global.LabelPrefix))
                            {
                                var label = labels.FirstOrDefault(l => l.Value == addressRegisters[index]);
                                dataReader.Position = (int)label.Key;

                                if (subRoutine)
                                    RenameLabel(addressRegisters[index], addressRegisters[index].Replace(Global.LabelPrefix, Global.FunctionPrefix));
                            }
                            else if (addressRegisters[index].StartsWith(Global.FunctionPrefix))
                            {
                                var label = labels.FirstOrDefault(l => l.Value == addressRegisters[index]);
                                dataReader.Position = (int)label.Key;
                            }
                            else if (addressRegisters[index].StartsWith(Global.DataPrefix))
                            {
                                var label = labels.FirstOrDefault(l => l.Value == addressRegisters[index]);
                                dataReader.Position = (int)label.Key;

                                if (subRoutine)
                                    RenameLabel(addressRegisters[index], addressRegisters[index].Replace(Global.DataPrefix, Global.FunctionPrefix));
                                else
                                    RenameLabel(addressRegisters[index], addressRegisters[index].Replace(Global.DataPrefix, Global.LabelPrefix));
                            }
                        }

                        return false;
                    }

                    unresolvedJumps.Add(new UnresolvedJump(jumpTarget, jumpCallOffset));

                    // A jump to a sub-routine will always return at some time or at least is expected to, so continue after the instruction in this case.
                    // On the other hand a normal jump can not be expected to return, so just stop execution here.
                    if (!subRoutine) 
                        stop = true;
                }
            }

            void RenameLabel(string oldName, string newName)
            {
                var label = labels.FirstOrDefault(l => l.Value == oldName);

                if (label.Key != 0)
                    labels[label.Key] = newName;

                var reference = references[Index].FirstOrDefault(r => r.Label == oldName && r.SourceHunkIndex == Index && r.TargetHunkIndex == Index);

                if (reference is not null)
                {
                    references[Index].Remove(reference);
                    references[Index].Add(new Reference(newName, Index, reference.SourceOffset, Index, reference.RelativeOffset));
                }
            }

            void ReferenceHandler(Dictionary<string, opcodes.Reference> references)
            {
                foreach (var reference in references)
                {
                    if (!relocs.TryGetValue(reference.Value.UsageOffset, out int hunkIndex))
                    {
                        if (reference.Value.RelativeAddress == 0x00000004 || // special address
                            reference.Value.RelativeAddress >= 0x00bfd000 && reference.Value.RelativeAddress < 0x00bfef02 || // Amiga hardware registers
                            reference.Value.RelativeAddress >= 0x00dff000 && reference.Value.RelativeAddress < 0x00e00000) // Amiga hardware registers
                        {
                            currentAsmLabelReplacements.Add(reference.Key, $"#${reference.Value.RelativeAddress:x8}");
                            continue;
                        }

                        throw new InvalidDataException($"Missing reloc entry for reference in hunk {Index} at offset ${reference.Value.RelativeAddress:x8}.");
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
