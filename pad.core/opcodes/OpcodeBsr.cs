using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// BSR - Branch to subroutine
    /// 
    /// BSR &lt;label&gt;
    /// Size: Byte, Word
    /// </summary>
    internal class OpcodeBsr : BaseBranchOpcode
    {
        public OpcodeBsr()
            : base(0xff00, 0x6100, ToAsm, true, header => (header & 0x00ff) == 0 ? 4 : 2)
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

            return $"BSR.{suffix} <LABEL>";
        }
    }
}
