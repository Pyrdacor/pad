using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// CMP - Compare
    /// 
    /// CMP &lt;ea&gt;,Dn
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeCmp : BaseOpcode
    {
        public OpcodeCmp()
            : base(IsMatch, ToAsm, header => SizeWithArg(header, 2 * ((header >> 6) & 0x3)))
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xf100) != 0xb000)
                return false;

            if ((header & 0xc0) == 0xc0) // would be CPMA
                return false;

            return true;
        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var reg = (header >> 9) & 0x7;

            Tuple<int, string> info = ((header >> 6) & 0x3) switch
            {
                0 => Tuple.Create(1, "B"),
                1 => Tuple.Create(2, "W"),
                2 => Tuple.Create(4, "L"),
                _ => throw new InvalidDataException("Invalid CMP instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, AddressingModes.All);

            return KeyValuePair.Create($"CMP.{info.Item2} {arg},D{reg}", addresses);
        }
    }
}
