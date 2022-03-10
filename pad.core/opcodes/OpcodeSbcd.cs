using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// SBCD - Subtract decimal with extend
    /// 
    /// SBCD Dy,Dx
    /// SBCD -(Ay),-(Ax)
    /// Size: Byte
    /// </summary>
    internal class OpcodeSbcd : BaseOpcode
    {
        public OpcodeSbcd()
            : base(IsMatch, ToAsm)
        {

        }

        static bool IsMatch(ushort header)
        {
            header &= 0xf1f8;

            return header == 0x8100 || header == 0x8108;
        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            var dst = (header >> 9) & 0x7;
            var src = header & 0x7;

            if ((header & 0x80) == 0) // Dx <- Dy
                return $"SBCD D{src},D{dst}";
            else // -(Ax) <- -(Ay)
                return $"SBCD -({Global.AddressRegisterName(src)}),-({Global.AddressRegisterName(dst)})";
        }
    }
}
