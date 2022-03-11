using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// SUBA - Subtract address
    /// 
    /// SUBA &lt;ea&gt;,An
    /// Size: Word, Long
    /// </summary>
    internal class OpcodeSuba : BaseOpcode
    {
        public OpcodeSuba()
            : base(0xf0c0, 0x90c0, ToAsm, header => SizeWithArg(header, 2 + 2 * ((header >> 8) & 0x1)))
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

            return KeyValuePair.Create($"SUBA.{info.Item2} {arg},{Global.AddressRegisterName(reg)}", addresses);
        }
    }
}
