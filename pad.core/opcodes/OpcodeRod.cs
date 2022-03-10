using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ROL, ROR - Rotate left/right (data register versions)
    /// 
    /// ROL Dx,Dy
    /// ROR Dx, Dy
    /// ROL #&lt;data&gt;,Dy
    /// ROR #&lt;data&gt;,Dy
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeRod : BaseOpcode
    {
        public OpcodeRod()
            : base(IsMatch, ToAsm)
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xf018) != 0xe018)
                return false;

            if ((header & 0x00c0) == 0x00c0) // would be the memory version of ROd
                return false;

            return true;
        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            string dir = (header & 0x0100) == 0 ? "R" : "L";
            var addresses = new List<uint>();
            var amount = (header >> 9) & 0x7;
            Tuple<string, string> info = ((header >> 6) & 0x3) switch
            {
                0 => Tuple.Create("B", "B"),
                1 => Tuple.Create("W", "W"),
                2 => Tuple.Create("L", ""),
                _ => throw new InvalidDataException("Invalid ROd instruction.")
            };
            string amountStr = (header & 0x20) == 0
                ? $"#${amount:x2}"
                : $"D{amount}";
            var reg = header & 0x7;

            return KeyValuePair.Create($"RO{dir}.{info.Item1} {amountStr},D{reg}{info.Item2}", addresses);
        }
    }
}
