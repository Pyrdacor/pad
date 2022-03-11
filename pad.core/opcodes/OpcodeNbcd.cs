using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// NBCD - Negate decimal with sign extend
    /// 
    /// NBCD &lt;ea&gt;
    /// Size: Byte
    /// </summary>
    internal class OpcodeNbcd : BaseOpcode
    {
        public OpcodeNbcd()
            : base(0xffc0, 0x4800, ToAsm, header => SizeWithArg(header, 0))
        {

        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var arg = ParseArg(header, 10, dataReader, 1, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"NBCD {arg}", addresses);
        }
    }
}
