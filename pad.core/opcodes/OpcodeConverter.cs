using pad.core.interfaces;

namespace pad.core.opcodes
{
    public class OpcodeConverter
    {
        static readonly Dictionary<byte, IOpcode[]> OpcodesBy4BitHeader = new();
        static readonly Dictionary<byte, IOpcode> OpcodesBy8BitHeader = new();
        static readonly Dictionary<ushort, IOpcode> OpcodesBy10BitHeader = new();
        static readonly Dictionary<ushort, IOpcode> OpcodesBy12BitHeader = new();
        static readonly Dictionary<ushort, IOpcode> OpcodesBy13BitHeader = new();
        static readonly Dictionary<ushort, ISimple16BitOpcode> OpcodesBy16BitHeader = new();

        static OpcodeConverter()
        {
            // Plain 16 bit opcodes
            OpcodesBy16BitHeader.Add(0x003c, new OpcodeOriToCcr());
            OpcodesBy16BitHeader.Add(0x007c, new OpcodeOriToSr());
            OpcodesBy16BitHeader.Add(0x023c, new OpcodeAndiToCcr());
            OpcodesBy16BitHeader.Add(0x027c, new OpcodeAndiToSr());
            OpcodesBy16BitHeader.Add(0x0a3c, new OpcodeEoriToCcr());
            OpcodesBy16BitHeader.Add(0x0a7c, new OpcodeEoriToSr());
            OpcodesBy16BitHeader.Add(0x4afc, new OpcodeIllegal());
            OpcodesBy16BitHeader.Add(0x4e70, new OpcodeReset());
            OpcodesBy16BitHeader.Add(0x4e71, new OpcodeNop());
            OpcodesBy16BitHeader.Add(0x4e72, new OpcodeStop());
            OpcodesBy16BitHeader.Add(0x4e73, new OpcodeRte());
            OpcodesBy16BitHeader.Add(0x4e75, new OpcodeRts());
            OpcodesBy16BitHeader.Add(0x4e76, new OpcodeTrapv());
            OpcodesBy16BitHeader.Add(0x4e77, new OpcodeRtr());

            // Opcodes with fixed 13 bit header
            OpcodesBy13BitHeader.Add(0x4840, new OpcodeSwap());
            OpcodesBy13BitHeader.Add(0x4880, new OpcodeExt());
            OpcodesBy13BitHeader.Add(0x48c0, new OpcodeExt());
            OpcodesBy13BitHeader.Add(0x4e50, new OpcodeLink());
            OpcodesBy13BitHeader.Add(0x4e58, new OpcodeUnlnk());
            OpcodesBy13BitHeader.Add(0x4e60, new OpcodeMoveUsp());
            OpcodesBy13BitHeader.Add(0x4e68, new OpcodeMoveUsp());

            // Opcodes with fixed 12 bit header
            OpcodesBy12BitHeader.Add(0x4e40, new OpcodeTrap());

            // Opcodes with fixed 10 bit header
            OpcodesBy10BitHeader.Add(0x0800, new OpcodeBtsti());
            OpcodesBy10BitHeader.Add(0x0840, new OpcodeBchgi());
            OpcodesBy10BitHeader.Add(0x0880, new OpcodeBclri());
            OpcodesBy10BitHeader.Add(0x08c0, new OpcodeBseti());
            OpcodesBy10BitHeader.Add(0x40c0, new OpcodeMoveFromSr());
            OpcodesBy10BitHeader.Add(0x44c0, new OpcodeMoveToCcr());
            OpcodesBy10BitHeader.Add(0x46c0, new OpcodeMoveToSr());
            OpcodesBy10BitHeader.Add(0x4800, new OpcodeNbcd());
            OpcodesBy10BitHeader.Add(0x4840, new OpcodePea());
            OpcodesBy10BitHeader.Add(0x4ac0, new OpcodeTas());
            OpcodesBy10BitHeader.Add(0x4e80, new OpcodeJsr());
            OpcodesBy10BitHeader.Add(0x4ec0, new OpcodeJmp());
            OpcodesBy10BitHeader.Add(0x4880, new OpcodeMovem());
            OpcodesBy10BitHeader.Add(0x48c0, new OpcodeMovem());
            OpcodesBy10BitHeader.Add(0x4c80, new OpcodeMovem());
            OpcodesBy10BitHeader.Add(0x4cc0, new OpcodeMovem());

            // Opcodes with fixed 8 bit header
            OpcodesBy8BitHeader.Add(0x00, new OpcodeOri());
            OpcodesBy8BitHeader.Add(0x02, new OpcodeAndi());
            OpcodesBy8BitHeader.Add(0x04, new OpcodeSubi());
            OpcodesBy8BitHeader.Add(0x06, new OpcodeAddi());
            OpcodesBy8BitHeader.Add(0x0a, new OpcodeEori());
            OpcodesBy8BitHeader.Add(0x0c, new OpcodeCmpi());
            OpcodesBy8BitHeader.Add(0x40, new OpcodeNegX());
            OpcodesBy8BitHeader.Add(0x42, new OpcodeClr());
            OpcodesBy8BitHeader.Add(0x44, new OpcodeNeg());
            OpcodesBy8BitHeader.Add(0x46, new OpcodeNot());
            OpcodesBy8BitHeader.Add(0x4a, new OpcodeTst());

            // Opcodes with fixed 4 bit header
            OpcodesBy4BitHeader.Add(0x00, new IOpcode[]
            {
                new OpcodeBtst(), new OpcodeBchg(), new OpcodeBclr(), new OpcodeBset(), new OpcodeMovep()
            });
            OpcodesBy4BitHeader.Add(0x10, new IOpcode[]
            {
                new OpcodeMovea(), new OpcodeMove()
            });
            OpcodesBy4BitHeader.Add(0x20, new IOpcode[]
            {
                new OpcodeMovea(), new OpcodeMove()
            });
            OpcodesBy4BitHeader.Add(0x30, new IOpcode[]
            {
                new OpcodeMovea(), new OpcodeMove()
            });
            OpcodesBy4BitHeader.Add(0x40, new IOpcode[]
            {
                new OpcodeLea(), new OpcodeChk()
            });
            OpcodesBy4BitHeader.Add(0x50, new IOpcode[]
            {
                new OpcodeAddq(), new OpcodeSubq(), new OpcodeScc(), new OpcodeDbcc()
            });
            OpcodesBy4BitHeader.Add(0x60, new IOpcode[]
            {
                new OpcodeBra(), new OpcodeBsr(), new OpcodeBcc()
            });
            OpcodesBy4BitHeader.Add(0x70, new IOpcode[]
            {
                new OpcodeMoveq()
            });
            OpcodesBy4BitHeader.Add(0x80, new IOpcode[]
            {
                new OpcodeDivu(), new OpcodeDivs(), new OpcodeSbcd(), new OpcodeOr()
            });
            OpcodesBy4BitHeader.Add(0x90, new IOpcode[]
            {
                new OpcodeSub(), new OpcodeSubx(), new OpcodeSuba()
            });
            OpcodesBy4BitHeader.Add(0xb0, new IOpcode[]
            {
                new OpcodeEor(), new OpcodeCmpm(), new OpcodeCmp(), new OpcodeCmpa()
            });
            OpcodesBy4BitHeader.Add(0xc0, new IOpcode[]
            {
                new OpcodeMulu(), new OpcodeMuls(), new OpcodeAbcd(), new OpcodeAnd()
            });
            OpcodesBy4BitHeader.Add(0xd0, new IOpcode[]
            {
                new OpcodeAdd(), new OpcodeAddx(), new OpcodeAdda()
            });
        }

        public void Convert(IDataReader dataReader, Action<string> asmOutputHandler)
        {
            while (dataReader.Position < dataReader.Size)
            {
                if (OpcodesBy16BitHeader.TryGetValue(dataReader.PeekWord(), out var opcode16Bit))
                {
                    asmOutputHandler(opcode16Bit.ConvertToAsm(dataReader));
                }
                else
                {
                    void ProcessOpcode(IOpcode opcode)
                    {
                        this.ProcessOpcode(opcode, dataReader, asmOutputHandler);
                    }

                    ushort code = dataReader.PeekWord();
                    ushort mask = (ushort)(code & 0xfff8); // 13 bit mask

                    if (OpcodesBy13BitHeader.TryGetValue(mask, out var opcode))
                    {
                        ProcessOpcode(opcode);
                        continue;
                    }

                    mask &= 0xfff0; // 12 bit mask

                    if (OpcodesBy12BitHeader.TryGetValue(mask, out opcode))
                    {
                        ProcessOpcode(opcode);
                        continue;
                    }

                    mask &= 0xffc0; // 10 bit mask

                    if (OpcodesBy10BitHeader.TryGetValue(mask, out opcode))
                    {
                        ProcessOpcode(opcode);
                        continue;
                    }

                    byte bMask = (byte)(mask >> 8); // 8 bit mask

                    if (OpcodesBy8BitHeader.TryGetValue(bMask, out opcode))
                    {
                        ProcessOpcode(opcode);
                        continue;
                    }

                    bMask &= 0xf0;

                    if (!OpcodesBy4BitHeader.TryGetValue(bMask, out var opcodes))
                        throw new InvalidDataException($"Invalid opcode {code:x4}");

                    ProcessOpcode(ReadOpcode(opcodes, dataReader));
                }
            }
        }

        IOpcode ReadOpcode(IEnumerable<IOpcode> possibleOpcodes, IDataReader dataReader)
        {

        }

        void ProcessOpcode(IOpcode opcode, IDataReader dataReader, Action<string> asmOutputHandler)
        {
            if (!opcode.TryMatch(dataReader, out string asm, out var absoluteLongAddresses))
                throw new InvalidDataException("Invalid opcode data.");

            asmOutputHandler(asm);
        }
    }
}
