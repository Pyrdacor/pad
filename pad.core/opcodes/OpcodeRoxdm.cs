using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ROXL, ROXR - Rotate left/right with extend (memory version)
    /// 
    /// ROXL &lt;ea&gt;
    /// ROXR &lt;ea&gt;
    /// Size: Word
    /// </summary>
    internal class OpcodeRoxdm : BaseOpcode
    {
        public OpcodeRoxdm()
            : base(0xfec0, 0xe4c0, ToAsm, header => SizeWithArg(header, 2))
        {

        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            string dir = (header & 0x0100) == 0 ? "R" : "L";
            var addresses = new Dictionary<string, Reference>();
            var arg = ParseArg(header, 10, dataReader, 2, addresses, AddressingModes.Memory);

            return KeyValuePair.Create($"ROX{dir}.W {arg}", addresses);
        }
    }
}
