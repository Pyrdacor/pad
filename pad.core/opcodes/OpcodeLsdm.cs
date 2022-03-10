using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// LSL, LSR - Logical shift left/right (memory version)
    /// 
    /// LSL &lt;ea&gt;
    /// LSR &lt;ea&gt;
    /// Size: Word
    /// </summary>
    internal class OpcodeLsdm : BaseOpcode
    {
        public OpcodeLsdm()
            : base(0xfec0, 0xe2c0, ToAsm)
        {

        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            string dir = (header & 0x0100) == 0 ? "R" : "L";
            var addresses = new List<uint>();
            var arg = ParseArg(header, 10, dataReader, 2, addresses, AddressingModes.Memory, "W");

            return KeyValuePair.Create($"LS{dir}.W {arg}", addresses);
        }
    }
}
