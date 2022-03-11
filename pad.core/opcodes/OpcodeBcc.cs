using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// Bcc - Branch on condition cc
    /// 
    /// BCC: branch on carry clear
    /// BCS: branch on carry set
    /// BEQ: branch on equal
    /// BGE: branch on greater than or equal
    /// BGT: branch on greater than
    /// BHI: branch on higher than
    /// BLE: branch on less than or equal
    /// BLS: branch on lower than or same
    /// BLT: branch on less than
    /// BMI: branch on minus
    /// BNE: branch on not equal
    /// BPL: branch on plus
    /// BVC: branch on overflow clear
    /// BVS: branch on overflow set
    /// 
    /// Bcc &lt;label&gt;
    /// Size: Byte, Word
    /// </summary>
    internal class OpcodeBcc : BaseBranchOpcode
    {
        public OpcodeBcc()
            : base(IsMatch, ToAsm, false, header => (header & 0x00ff) == 0 ? 4 : 2)
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xf000) != 0x6000)
                return false;

            return ((header >> 8) & 0xf) > 1; // otherwise would be BRA or BSR
        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            string suffix = "B";
            ushort d = (ushort)(header & 0x00ff);
            var condition = (Condition)((header >> 8) & 0xf);

            if (d == 0)
            {
                suffix = "W";
                dataReader.Position += 2;
            }

            return $"B{condition}.{suffix} <LABEL>";
        }
    }
}
