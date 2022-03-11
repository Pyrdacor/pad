using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ADDX - Add extended
    /// 
    /// ADDX Dy,Dx
    /// ADDX -(Ay),-(Ax)
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeAddx : BaseOpcode
    {
        public OpcodeAddx()
            : base(IsMatch, ToAsm, _ => 2)
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xf130) != 0xd100)
                return false;

            if ((header & 0x00c0) == 0x00c0) // would be ADDA
                return false;

            return true;
        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            var dst = (header >> 9) & 0x7;
            var src = header & 0x7;
            string suffix = ((header >> 6) & 0x3) switch
            {
                0 => "B",
                1 => "W",
                2 => "L",
                _ => throw new InvalidDataException("Invalid SUBX instruction.")
            };

            if ((header & 0x80) == 0) // Dx <- Dy
                return $"ADDX.{suffix} D{src},D{dst}";
            else // -(Ax) <- -(Ay)
                return $"ADDX.{suffix} -({Global.AddressRegisterName(src)}),-({Global.AddressRegisterName(dst)})";
        }
    }
}
