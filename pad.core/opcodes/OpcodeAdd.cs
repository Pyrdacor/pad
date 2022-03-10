using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ADD - Add binary
    /// 
    /// ADD &lt;ea&gt;,Dn
    /// ADD Dn,&lt;ea&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeAdd : BaseOpcode
    {
        public OpcodeAdd()
            : base(IsMatch, ToAsm, header => SizeWithArg(header, 2 * ((header >> 6) & 0x3)))
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header >> 12) != 0xd)
                return false;

            if ((header & 0x00c0) == 0x00c0) // would be ADDA
                return false;

            if ((header & 0xf130) == 0xd100) // would be ADDX
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
                _ => throw new InvalidDataException("Invalid ADD instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, addressingModes, info.Item3);

            return toDataRegister
                ? KeyValuePair.Create($"ADD.{info.Item2} {arg},D{reg}{info.Item3}", addresses)
                : KeyValuePair.Create($"ADD.{info.Item2} D{reg}{info.Item3},{arg}", addresses);
        }
    }
}
