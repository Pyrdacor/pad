using pad.core.extensions;
using pad.core.interfaces;
using pad.core.util;

namespace pad.core.opcodes
{
    /// <summary>
    /// MOVEQ - Move quick (copy a small literal to a destination)
    /// 
    /// MOVEQ #&lt;data&gt;,Dn
    /// Size: Long
    /// </summary>
    internal class OpcodeMoveq : BaseOpcode
    {
        public OpcodeMoveq()
            : base(0xf100, 0x7000, ToAsm)
        {

        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            var reg = (header >> 9) & 0x7;
            var value = ByteConverter.AsSigned((byte)(header & 0xff)).ToSignedHexString();

            return $"MOVEQ #{value},D{reg}";
        }
    }
}
