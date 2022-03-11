using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// CHK - Check register against bounds
    /// 
    /// CHK &lt;ea&gt;,Dn
    /// Size: Word
    /// </summary>
    internal class OpcodeChk : BaseOpcode
    {
        public OpcodeChk()
            : base(0xf1c0, 0x41a0, ToAsm, header => SizeWithArg(header, 0))
        {

        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var reg = (header >> 9) & 0x7;
            var arg = ParseArg(header, 10, dataReader, 4, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"CHK {arg},D{reg}", addresses);
        }
    }
}
