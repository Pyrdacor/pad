using pad.core.extensions;
using pad.core.interfaces;
using pad.core.util;

namespace pad.core.opcodes
{
    internal abstract class BaseOpcode : IOpcode
    {
        readonly Func<ushort, int> binarySizeProvider;
        readonly Func<ushort, bool> matcher;
        readonly Func<ushort, IDataReader, KeyValuePair<string, Dictionary<string, uint>>> asmProvider;

        protected BaseOpcode(Func<ushort, bool> matcher, Func<ushort, IDataReader, string> asmProvider, Func<ushort, int> binarySizeProvider)
        {
            this.binarySizeProvider = binarySizeProvider;
            this.matcher = matcher;
            this.asmProvider = (header, reader) =>
            {
                return KeyValuePair.Create(asmProvider(header, reader), new Dictionary<string, uint>());
            };
        }

        protected BaseOpcode(ushort mask, ushort value, Func<ushort, IDataReader, string> asmProvider, Func<ushort, int> binarySizeProvider)
        {
            this.binarySizeProvider = binarySizeProvider;
            matcher = v => (v & mask) == value;
            this.asmProvider = (header, reader) =>
            {
                return KeyValuePair.Create(asmProvider(header, reader), new Dictionary<string, uint>());
            };
        }

        protected BaseOpcode(Func<ushort, bool> matcher, Func<ushort, IDataReader, KeyValuePair<string, Dictionary<string, uint>>> asmProvider,
            Func<ushort, int> binarySizeProvider)
        {
            this.binarySizeProvider = binarySizeProvider;
            this.matcher = matcher;
            this.asmProvider = asmProvider;
        }

        protected BaseOpcode(ushort mask, ushort value, Func<ushort, IDataReader, KeyValuePair<string, Dictionary<string, uint>>> asmProvider,
            Func<ushort, int> binarySizeProvider)
        {
            this.binarySizeProvider = binarySizeProvider;
            matcher = v => (v & mask) == value;
            this.asmProvider = asmProvider;
        }

        public virtual bool TryMatch(IDataReader reader, out string asm, out Dictionary<string, uint> references, out int binarySize)
        {
            if (matcher(reader.PeekWord()))
            {
                ushort header = reader.ReadWord();
                var result = asmProvider(header, reader);

                asm = result.Key;
                references = result.Value;
                binarySize = binarySizeProvider(header);

                return true;
            }

            asm = "";
            references = new();
            binarySize = 0;

            return false;
        }

        protected static int SizeWithArg(ushort header, int immediateBytes, int bitOffset = 10)
        {
            if (bitOffset < 0 || bitOffset > 16 - 6)
                throw new ArgumentOutOfRangeException(nameof(bitOffset), "Bit index was out of range.");

            var bits = header >> (16 - 6 - bitOffset);

            var headBits = (bits >> 3) & 0x7;

            if (headBits == 5 | headBits == 6)
                return 4;

            if (headBits != 7)
                return 2;

            var regBits = bits & 0x7;

            return regBits switch
            {
                0 => 4,
                1 => 6,
                2 => 4,
                3 => 4,
                4 => 2 + Math.Max(2, immediateBytes),
                _ => throw new InvalidDataException("Invalid address mode.")
            };
        }

        /// <summary>
        /// Reads the 6 bits which specifies a source or target parameter.
        /// For registers, the last 3 bits give the register index.
        /// If reversed is set, the register bits come first (only used by the MOVE opcode).
        /// </summary>
        protected static string ParseArg(ushort header, int bitOffset, IDataReader dataReader,
            int immediateBytes, Dictionary<string, uint> references, AddressingModes addressingModes,
            string typeSuffix = "", bool reversed = false, bool jump = false)
        {
            if (bitOffset < 0 || bitOffset > 16 - 6)
                throw new ArgumentOutOfRangeException(nameof(bitOffset), "Bit index was out of range.");

            var bits = header >> (16 - 6 - bitOffset);

            var headBits = reversed ? bits & 0x7 : (bits >> 3) & 0x7;
            var regBits = reversed ? (bits >> 3) & 0x7 : bits & 0x7;

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

            addressingMode = (1 << addressingMode);

            if (!addressingModes.HasFlag((AddressingModes)addressingMode))
                throw new InvalidDataException($"Addressing mode {(AddressingModes)addressingMode} is not supported by this opcode.");

            string HandleAbsoluteLongReference()
            {
                uint address = dataReader.ReadDword();

                if ((address & 0xff000000) != 0)
                    throw new InvalidDataException("Absolute addresses must not exceed 24bit.");

                string label = jump
                    ? string.Format(Global.FirstPassLabelFormatString, address)
                    : string.Format(Global.FirstPassDataFormatString, address);

                references.Add(label, (uint)(dataReader.Position - 4));

                return label + ".l";
            }

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
                    1 => HandleAbsoluteLongReference(), // Absolute long address, TODO: Use label names?
                    2 => $"(${dataReader.ReadDisplacement()},PC)", // Program counter with displacement
                    3 => ReadAddressWithIndex(true), // Program counter with index
                    4 => immediateBytes switch
                    {
                        1 => $"#${dataReader.ReadWord() & 0xff:x2}",
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
