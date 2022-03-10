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
            : base(IsMatch, ToAsm)
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

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new List<uint>();
            var reg = (header >> 9) & 0x7;

            Tuple<int, string, string> info = ((header >> 6) & 0x3) switch
            {
                0 => Tuple.Create(1, "B", "B"),
                1 => Tuple.Create(2, "W", "W"),
                2 => Tuple.Create(4, "L", ""),
                _ => throw new InvalidDataException("Invalid CMP instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, AddressingModes.All, info.Item3);

            return KeyValuePair.Create($"CMP.{info.Item2} {arg},D{reg}{info.Item3}", addresses);
        }
    }
}
