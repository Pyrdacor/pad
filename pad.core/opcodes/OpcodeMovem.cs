using pad.core.extensions;
using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// MOVEM - Move multiple registers
    /// 
    /// MOVEM &lt;ea&gt;,&lt;register list&gt;
    /// MOVEM &lt;register list&gt;,&lt;ea&gt;
    /// Size: Word, Long
    /// </summary>
    internal class OpcodeMovem : BaseOpcode
    {
        public OpcodeMovem()
            : base(0xfb80, 0x4880, ToAsm, header => SizeWithArg(header, 0))
        {

        }

        // TODO: This is bugged
        static string RegisterList(ushort mask, bool toRegister)
        {
            string result = "";
            List<int> currentRegs = new(8);
            int inc = toRegister ? 1 : -1;
            int start = toRegister ? 0 : 15;

            for (int i = 0; i < 16; ++i)
            {
                int index = start + i * inc;

                string GetRegName(int reg) => i > 8 ? $"A{reg}" : $"D{reg}";

                if (i == 8 || // transition from D to A
                    (mask & (1 << index)) == 0)
                {
                    if (currentRegs.Count != 0)
                    {
                        if (result.Length != 0)
                            result += "/";

                        if (currentRegs.Count == 1)
                            result += GetRegName(currentRegs[0]);
                        else
                            result += GetRegName(currentRegs[0]) + "-" + GetRegName(currentRegs[^1]);

                        currentRegs.Clear();
                    }

                    if (i == 8 && (mask & (1 << index)) != 0)
                        currentRegs.Add(0);
                }
                else
                {
                    currentRegs.Add(i % 8);
                }
            }

            if (currentRegs.Count != 0)
            {
                if (result.Length != 0)
                    result += "/";

                if (currentRegs.Count == 1)
                    result += $"A{currentRegs[0]}";
                else
                    result += $"A{currentRegs[0]}-{currentRegs[^1]}";
            }

            return result;
        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            bool toRegister = ((header >> 10) & 0x1) != 0;
            var registerList = RegisterList(dataReader.ReadWord(), toRegister);

            if (registerList.Length == 0)
                throw new InvalidDataException("No register was given for MOVEM instruction.");

            var addresses = new Dictionary<string, Reference>();
            AddressingModes addressingModes = toRegister
                ? AddressingModes.All.Exclude(AddressingModes.DataRegister, AddressingModes.AddressRegister, AddressingModes.AddressWithPre, AddressingModes.Immediate)
                : AddressingModes.All.Exclude(AddressingModes.DataRegister, AddressingModes.AddressRegister, AddressingModes.AddressWithPost, AddressingModes.Immediate,
                  AddressingModes.PCWithDisplacement, AddressingModes.PCWithIndex);
            Tuple<int, string> info = (header & 0x0040) == 0
                ? Tuple.Create(2, "W")
                : Tuple.Create(4, "L");
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, addressingModes);

            string asm = toRegister
                ? $"MOVEM.{info.Item2} {arg},{registerList}"
                : $"MOVEM.{info.Item2} {registerList},{arg}";

            return KeyValuePair.Create(asm, addresses);
        }
    }
}
