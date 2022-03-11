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
            : base(0xfec0, 0xe6c0, ToAsm, header => SizeWithArg(header, 2))
        {

        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            string dir = (header & 0x0100) == 0 ? "R" : "L";
            var addresses = new Dictionary<string, Reference>();
            var arg = ParseArg(header, 10, dataReader, 2, addresses, AddressingModes.Memory);

            return KeyValuePair.Create($"RO{dir}.W {arg}", addresses);
        }
    }
}
