using pad.core.extensions;
using pad.core.interfaces;
using pad.core.util;

namespace pad.core.opcodes
{
    /// <summary>
    /// BRA - Branch always
    /// 
    /// BRA &lt;label&gt;
    /// Size: Byte, Word
    /// </summary>
    internal class OpcodeBra : BaseBranchOpcode
    {
        public OpcodeBra()
            : base(0xff00, 0x6000, ToAsm)
        {

        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            string suffix = "B";
            ushort d = (ushort)(header & 0x00ff);
            string displacement;

            if (d == 0)
            {
                suffix = "W";
                displacement = dataReader.ReadDisplacement();
            }
            else
            {
                displacement = ByteConverter.AsSigned((byte)d).ToSignedHexString();
            }

            return $"BRA.{suffix} #{displacement}";
        }
    }
}
