using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ABCD - Add decimal with extend
    /// 
    /// ABCD Dy,Dx
    /// ABCD -(Ay),-(Ax)
    /// Size: Byte
    /// </summary>
    internal class OpcodeAbcd : BaseOpcode
    {
        public OpcodeAbcd()
            : base(IsMatch, ToAsm, _ => 2)
        {

        }

        static bool IsMatch(ushort header)
        {
            header &= 0xf1f8;

            return header == 0xc100 || header == 0xc108;
        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            var dst = (header >> 9) & 0x7;
            var src = header & 0x7;

            if ((header & 0x80) == 0) // Dx <- Dy
                return $"ABCD D{src},D{dst}";
            else // -(Ax) <- -(Ay)
                return $"ABCD -({Global.AddressRegisterName(src)}),-({Global.AddressRegisterName(dst)})";
        }
    }
}
