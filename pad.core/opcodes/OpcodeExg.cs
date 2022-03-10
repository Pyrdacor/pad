using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// EXG - Add decimal with extend
    /// 
    /// EXG Dx,Dy
    /// EXG Ax,Ay
    /// EXG Dx,Ay
    /// EXG Ax,Dy
    /// Size: Long
    /// </summary>
    internal class OpcodeExg : BaseOpcode
    {
        public OpcodeExg()
            : base(IsMatch, ToAsm)
        {

        }

        static bool IsMatch(ushort header)
        {
            var opmode = (header >> 3) & 0x1f;
            header &= 0xf130;

            if (header != 0xc100)
                return false;

            return opmode == 0x08 || opmode == 0x09 || opmode == 0x11;
        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            var dst = (header >> 9) & 0x7;
            var src = header & 0x7; // if Dx, Ay this is always the Ay
            var opmode = (header >> 3) & 0x1f;

            return opmode switch
            {
                0x08 => $"EXG D{src},D{dst}", // Dx, Dy
                0x09 => $"EXG {Global.AddressRegisterName(src)},{Global.AddressRegisterName(dst)}", // Ax, Ay
                0x11 => $"EXG D{dst},{Global.AddressRegisterName(src)}", // Dx, Ay (also Ax, Dy)
                _ => throw new InvalidDataException("Invalid EXG instruction.")
            };
        }
    }
}
