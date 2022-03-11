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
            : base(IsMatch, ToAsm, _ => 2)
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

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            string dir = (header & 0x0100) == 0 ? "R" : "L";
            var amount = (header >> 9) & 0x7;
            string suffix = ((header >> 6) & 0x3) switch
            {
                0 => "B",
                1 => "W",
                2 => "L",
                _ => throw new InvalidDataException("Invalid ASd instruction.")
            };
            string amountStr = (header & 0x20) == 0
                ? $"#${amount:x2}"
                : $"D{amount}";
            var reg = header & 0x7;

            return $"AS{dir}.{suffix} {amountStr},D{reg}";
        }
    }
}
