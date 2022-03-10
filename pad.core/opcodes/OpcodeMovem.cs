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
            : base(0xfb80, 0x4880, ToAsm)
        {

        }

        static string RegisterList(ushort mask, bool toRegister)
        {
            string result = "";
            List<int> currentRegs = new(8);
            int inc = toRegister ? - 1 : 1;
            int start = toRegister ? 15 : 0;

            for (int i = 0; i < 16; ++i)
            {
                int index = start + i * inc;

                string GetRegName(int reg) => index >= 8 ? Global.AddressRegisterName(reg) : $"D{reg}";

                if (index == 8 || // transition from D to A
                    (mask & (1 << index)) == 0)
                {
                    if (result.Length != 0)
                        result += "/";

                    if (currentRegs.Count == 1)
                        result += GetRegName(currentRegs[0]);
                    else if (currentRegs.Count > 1)
                        result += GetRegName(currentRegs[0]) + "-" + GetRegName(currentRegs[^1]);

                    currentRegs.Clear();
                }
                else
                {
                    currentRegs.Add(i);
                }
            }

            return result;
        }

        static KeyValuePair<string, Dictionary<string, uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            bool toRegister = ((header >> 10) & 0x1) != 0;
            var registerList = RegisterList(dataReader.ReadWord(), toRegister);

            if (registerList.Length == 0)
                throw new InvalidDataException("No register was given for MOVEM instruction.");

            var addresses = new Dictionary<string, uint>();
            AddressingModes addressingModes = toRegister
                ? AddressingModes.All.Exclude(AddressingModes.DataRegister, AddressingModes.AddressRegister, AddressingModes.AddressWithPre, AddressingModes.Immediate)
                : AddressingModes.All.Exclude(AddressingModes.DataRegister, AddressingModes.AddressRegister, AddressingModes.AddressWithPost, AddressingModes.Immediate,
                  AddressingModes.PCWithDisplacement, AddressingModes.PCWithIndex);
            Tuple<int, string, string> info = (header & 0x0040) == 0
                ? Tuple.Create(2, "W", "W")
                : Tuple.Create(4, "L", "");
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, addressingModes, info.Item3);

            string asm = toRegister
                ? $"MOVEM.{info.Item2} {arg},{registerList}"
                : $"MOVEM.{info.Item2} {registerList},{arg}";

            return KeyValuePair.Create(asm, addresses);
        }
    }
}
