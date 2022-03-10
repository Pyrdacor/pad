using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ASL, ASR - Arithmetic shift left/right (data register versions)
    /// 
    /// ASL Dx,Dy
    /// ASR Dx, Dy
    /// ASL #&lt;data&gt;,Dy
    /// ASR #&lt;data&gt;,Dy
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeAsd : BaseOpcode
    {
        public OpcodeAsd()
            : base(IsMatch, ToAsm)
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xf018) != 0xe000)
                return false;

            if ((header & 0x00c0) == 0x00c0) // would be the memory version of ASd
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
                _ => throw new InvalidDataException("Invalid ASd instruction.")
            };
            string amountStr = (header & 0x20) == 0
                ? $"#${amount:x2}"
                : $"D{amount}";
            var reg = header & 0x7;

            return KeyValuePair.Create($"AS{dir}.{info.Item1} {amountStr},D{reg}{info.Item2}", addresses);
        }
    }
}
