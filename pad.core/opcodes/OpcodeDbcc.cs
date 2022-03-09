using pad.core.extensions;
using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// DBcc - Test condition, decrement, and branch
    /// 
    /// DBCC: leave loop on carry clear
    /// DBCS: leave loop on carry set
    /// DBEQ: leave loop on equal
    /// DBGE: leave loop on greater than or equal
    /// DBGT: leave loop on greater than
    /// DBHI: leave loop on higher than
    /// DBLE: leave loop on less than or equal
    /// DBLS: leave loop on lower than or same
    /// DBLT: leave loop on less than
    /// DBMI: leave loop on minus
    /// DBNE: leave loop on not equal
    /// DBPL: leave loop on plus
    /// DBVC: leave loop on overflow clear
    /// DBVS: leave loop on overflow set
    /// DBF:  leave loop on false
    /// DBT:  leave loop on true
    /// 
    /// DBcc Dn,&lt;label&gt;
    /// Size: Word
    /// </summary>
    internal class OpcodeDbcc : BaseOpcode
    {
        public OpcodeDbcc()
            : base(IsMatch, ToAsm)
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xf0c0) != 0x50c0)
                return false;

            return ((header >> 3) & 0x7) == 0x1; // otherwise would be Scc
        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new List<uint>();
            var reg = header & 0x7;
            var condition = (Condition)((header >> 8) & 0xf);
            var displacement = dataReader.ReadDisplacement();

            return KeyValuePair.Create($"DB{condition} D{reg}W,#{displacement}", addresses);
        }
    }
}
