using pad.core.interfaces;

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
            : base(0xff00, 0x6000, ToAsm, true, header => (header & 0x00ff) == 0 ? 4 : 2)
        {

        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            string suffix = "B";
            ushort d = (ushort)(header & 0x00ff);

            if (d == 0)
            {
                suffix = "W";
                dataReader.Position += 2;
            }

            return $"BRA.{suffix} <LABEL>";
        }
    }
}
