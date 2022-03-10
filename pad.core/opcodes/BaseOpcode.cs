using pad.core.extensions;
using pad.core.interfaces;
using pad.core.util;

namespace pad.core.opcodes
{
    internal abstract class BaseOpcode : IOpcode
    {
        readonly Func<ushort, bool> matcher;
        readonly Func<ushort, IDataReader, KeyValuePair<string, List<uint>>> asmProvider;

        protected BaseOpcode(Func<ushort, bool> matcher, Func<ushort, IDataReader, string> asmProvider)
        {
            this.matcher = matcher;
            this.asmProvider = (header, reader) =>
            {
                return KeyValuePair.Create(asmProvider(header, reader), new List<uint>());
            };
        }

        protected BaseOpcode(ushort mask, ushort value, Func<ushort, IDataReader, string> asmProvider)
        {
            matcher = v => (v & mask) == value;
            this.asmProvider = (header, reader) =>
            {
                return KeyValuePair.Create(asmProvider(header, reader), new List<uint>());
            };
        }

        protected BaseOpcode(Func<ushort, bool> matcher, Func<ushort, IDataReader, KeyValuePair<string, List<uint>>> asmProvider)
        {
            this.matcher = matcher;
            this.asmProvider = asmProvider;
        }

        protected BaseOpcode(ushort mask, ushort value, Func<ushort, IDataReader, KeyValuePair<string, List<uint>>> asmProvider)
        {
            matcher = v => (v & mask) == value;
            this.asmProvider = asmProvider;
        }

        public virtual bool TryMatch(IDataReader reader, out string asm, out List<uint> absoluteLongAddresses)
        {
            if (matcher(reader.PeekWord()))
            {
                var result = asmProvider(reader.ReadWord(), reader);
                asm = result.Key;
                absoluteLongAddresses = result.Value;

                return true;
            }

            asm = "";
            absoluteLongAddresses = new();

            return false;
        }

        /// <summary>
        /// Reads the 6 bits which specifies a source or target parameter.
        /// For registers, the last 3 bits give the register index.
        /// If reversed is set, the register bits come first (only used by the MOVE opcode).
        /// </summary>
        protected static string ParseArg(ushort header, int bitOffset, IDataReader dataReader,
            int immediateBytes, List<uint> absoluteLongAddresses, AddressingModes addressingModes,
            string typeSuffix = "", bool reversed = false)
        {
            if (bitOffset < 0 || bitOffset > 16 - 6)
                throw new ArgumentOutOfRangeException("Bit index was out of range.");

            var bits = header >> (16 - 6 - bitOffset);

            var headBits = reversed ? bits & 0x7 : bits >> 3;
            var regBits = reversed ? bits >> 3 : bits & 0x7;

            if (headBits == 7)
            {
                if (regBits == 0) // Absolute short
                    throw new NotSupportedException("Absolute short addresses are not supported.");
                else if (regBits == 1) // Absolute long
                    absoluteLongAddresses.Add(dataReader.PeekDword());
            }

            string AName() => Global.AddressRegisterName(regBits);

            string ReadAddressWithIndex(bool pc)
            {
                // (ds,An,Xn)
                // (ds,PC,Xn)
                var extensionWord = dataReader.ReadWord();

                if ((extensionWord & 0x700) != 0)
                    throw new InvalidDataException("Invalid address with index data.");

                string index = (extensionWord & 0x8000) == 0 ? "D" : "A"; // data or address register stored in upper bit
                extensionWord &= 0x7fff;
                index += (extensionWord >> 12).ToString(); // register index
                bool word = (extensionWord & 0x800) == 0;
                if (index == "A7")
                {
                    index = "SP";

                    if (word)
                        throw new InvalidOperationException("Stack pointer should not be used with .W suffix.");
                }
                index += word ? "W" : "L"; // type of index register (word or long)
                sbyte displacement = ByteConverter.AsSigned((byte)(extensionWord & 0xff));
                string baseRegister = pc ? "PC" : $"A{regBits}";

                return $"({displacement.ToSignedHexString()},{baseRegister},{index})";
            }

            int addressingMode = headBits;

            if (headBits == 7)
                addressingMode += regBits;

            if (!addressingModes.HasFlag((AddressingModes)addressingMode))
                throw new InvalidDataException($"Addressing mode {(AddressingModes)addressingMode} is not supported by this opcode.");

            return headBits switch
            {
                0 => $"D{regBits}{typeSuffix}", // Data register
                1 => $"{AName()}", // Address register
                2 => $"({AName()})", // Address
                3 => $"({AName()})+", // Address with postincrement
                4 => $"-({AName()})", // Address with predecrement
                5 => $"(${dataReader.ReadDisplacement()},{AName()})", // Address with displacement
                6 => ReadAddressWithIndex(false), // Address with index
                7 => regBits switch
                {
                    0 => throw new NotSupportedException("Absolute short addresses are not supported."), // Absolute short address
                    1 => $"(${dataReader.ReadDword():x8}).l", // Absolute long address, TODO: Use label names?
                    2 => $"(${dataReader.ReadDisplacement()},PC)", // Program counter with displacement
                    3 => ReadAddressWithIndex(true), // Program counter with index
                    4 => immediateBytes switch
                    {
                        1 => $"#${dataReader.ReadByte():x2}",
                        2 => $"#${dataReader.ReadWord():x4}",
                        4 => $"#${dataReader.ReadDword():x8}",
                        _ => throw new InvalidDataException("Invalid argument data.")
                    },
                    _ => throw new InvalidDataException("Invalid argument data.")
                },
                _ => throw new InvalidDataException("Invalid argument data.")
            };
        }
    }
}
