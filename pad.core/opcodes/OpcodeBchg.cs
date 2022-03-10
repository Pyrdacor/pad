using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// BCHG - Test a bit and change
    /// 
    /// BCHG Dn,&lt;ea&gt;
    /// Size: Long
    /// </summary>
    internal class OpcodeBchg : BaseOpcode
    {
        public OpcodeBchg()
            : base(0xf1c0, 0x0140, ToAsm, header => SizeWithArg(header, 0))
        {

        }

        static KeyValuePair<string, Dictionary<string, uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, uint>();
            var reg = (header >> 9) & 0x7;
            var arg = ParseArg(header, 10, dataReader, 4, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"BCHG.L D{reg},{arg}", addresses);
        }
    }
}
