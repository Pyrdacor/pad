using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// BCLR - Test a bit and clear
    /// 
    /// BCLR Dn,&lt;ea&gt;
    /// Size: Long
    /// </summary>
    internal class OpcodeBclr : BaseOpcode
    {
        public OpcodeBclr()
            : base(0xf1c0, 0x0180, ToAsm, header => SizeWithArg(header, 0))
        {

        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var reg = (header >> 9) & 0x7;
            var arg = ParseArg(header, 10, dataReader, 4, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"BCLR.L D{reg},{arg}", addresses);
        }
    }
}
