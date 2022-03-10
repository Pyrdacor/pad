using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// LSL, LSR - Logical shift left/right (data register versions)
    /// 
    /// LSL Dx,Dy
    /// LSR Dx, Dy
    /// LSL #&lt;data&gt;,Dy
    /// LSR #&lt;data&gt;,Dy
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeLsd : BaseOpcode
    {
        public OpcodeLsd()
            : base(IsMatch, ToAsm)
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xf018) != 0xe008)
                return false;

            if ((header & 0x00c0) == 0x00c0) // would be the memory version of LSd
                return false;

            return true;
        }

        static KeyValuePair<string, Dictionary<string, uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            string dir = (header & 0x0100) == 0 ? "R" : "L";
            var addresses = new Dictionary<string, uint>();
            var amount = (header >> 9) & 0x7;
            Tuple<string, string> info = ((header >> 6) & 0x3) switch
            {
                0 => Tuple.Create("B", "B"),
                1 => Tuple.Create("W", "W"),
                2 => Tuple.Create("L", ""),
                _ => throw new InvalidDataException("Invalid LSd instruction.")
            };
            string amountStr = (header & 0x20) == 0
                ? $"#${amount:x2}"
                : $"D{amount}";
            var reg = header & 0x7;

            return KeyValuePair.Create($"LS{dir}.{info.Item1} {amountStr},D{reg}{info.Item2}", addresses);
        }
    }
}
