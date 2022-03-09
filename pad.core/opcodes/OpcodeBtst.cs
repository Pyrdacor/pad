using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// BTST - Test a bit
    /// 
    /// BTST Dn,&lt;ea&gt;
    /// Size: Long
    /// </summary>
    internal class OpcodeBtst : BaseOpcode
    {
        public OpcodeBtst()
            : base(0xf1c0, 0x0100, ToAsm)
        {

        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new List<uint>();
            var reg = (header >> 9) & 0x7;
            var arg = ParseArg(header, 10, dataReader, 4, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"BTST.L D{reg},{arg}", addresses);
        }
    }
}
