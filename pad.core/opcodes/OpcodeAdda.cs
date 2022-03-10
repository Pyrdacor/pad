using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ADDA - Add address
    /// 
    /// ADDA &lt;ea&gt;,An
    /// Size: Word, Long
    /// </summary>
    internal class OpcodeAdda : BaseOpcode
    {
        public OpcodeAdda()
            : base(0xf0c0, 0xd0c0, ToAsm)
        {

        }

        static KeyValuePair<string, Dictionary<string, uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, uint>();
            var reg = (header >> 9) & 0x7;
            Tuple<int, string, string> info = ((header >> 8) & 0x1) == 0
                ? Tuple.Create(2, "W", "W")
                : Tuple.Create(4, "L", "");
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, AddressingModes.All, info.Item3);

            return KeyValuePair.Create($"ADDA.{info.Item2} {arg},{Global.AddressRegisterName(reg)}", addresses);
        }
    }
}
