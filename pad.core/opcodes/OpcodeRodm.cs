using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ROL, ROR - Rotate left/right (memory version)
    /// 
    /// ROL &lt;ea&gt;
    /// ROR &lt;ea&gt;
    /// Size: Word
    /// </summary>
    internal class OpcodeRodm : BaseOpcode
    {
        public OpcodeRodm()
            : base(0xfec0, 0xe6c0, ToAsm)
        {

        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            string dir = (header & 0x0100) == 0 ? "R" : "L";
            var addresses = new List<uint>();
            var arg = ParseArg(header, 10, dataReader, 2, addresses, AddressingModes.Memory, "W");

            return KeyValuePair.Create($"RO{dir}.W {arg}", addresses);
        }
    }
}
