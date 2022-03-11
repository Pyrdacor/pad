using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// CMPA - Compare address
    /// 
    /// CMPA &lt;ea&gt;,An
    /// Size: Word, Long
    /// </summary>
    internal class OpcodeCmpa : BaseOpcode
    {
        public OpcodeCmpa()
            : base(0xf0c0, 0xb0c0, ToAsm, header => SizeWithArg(header, 2 + 2 * ((header >> 8) & 0x1)))
        {

        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var reg = (header >> 9) & 0x7;
            Tuple<int, string> info = ((header >> 8) & 0x1) == 0
                ? Tuple.Create(2, "W")
                : Tuple.Create(4, "L");
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, AddressingModes.All);

            return KeyValuePair.Create($"CMPA.{info.Item2} {arg},{Global.AddressRegisterName(reg)}", addresses);
        }
    }
}
