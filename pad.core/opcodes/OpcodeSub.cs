using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// SUB - Subtract binary
    /// 
    /// SUB &lt;ea&gt;,Dn
    /// SUB Dn,&lt;ea&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeSub : BaseOpcode
    {
        public OpcodeSub()
            : base(IsMatch, ToAsm, header => SizeWithArg(header, 2 * ((header >> 6) & 0x3)))
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header >> 12) != 0x9)
                return false;

            if ((header & 0x00c0) == 0x00c0) // would be SUBA
                return false;

            if ((header & 0xf130) == 0x9100) // would be SUBX
                return false;

            return true;
        }

        static KeyValuePair<string, Dictionary<string, uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, uint>();
            var reg = (header >> 9) & 0x7;
            bool toDataRegister = (header & 0x0100) == 0;
            var addressingModes = toDataRegister
                ? AddressingModes.All
                : AddressingModes.Default;
            Tuple<int, string, string> info = ((header >> 6) & 0x3) switch
            {
                0 => Tuple.Create(1, "B", "B"),
                1 => Tuple.Create(2, "W", "W"),
                2 => Tuple.Create(4, "L", ""),
                _ => throw new InvalidDataException("Invalid SUB instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, addressingModes, info.Item3);

            return toDataRegister
                ? KeyValuePair.Create($"SUB.{info.Item2} {arg},D{reg}{info.Item3}", addresses)
                : KeyValuePair.Create($"SUB.{info.Item2} D{reg}{info.Item3},{arg}", addresses);
        }
    }
}
