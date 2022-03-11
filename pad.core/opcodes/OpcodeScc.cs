using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// Scc - Set according to condition cc
    /// 
    /// SCC: set on carry clear
    /// SCS: set on carry set
    /// SEQ: set on equal
    /// SGE: set on greater than or equal
    /// SGT: set on greater than
    /// SHI: set on higher than
    /// SLE: set on less than or equal
    /// SLS: set on lower than or same
    /// SLT: set on less than
    /// SMI: set on minus
    /// SNE: set on not equal
    /// SPL: set on plus
    /// SVC: set on overflow clear
    /// SVS: set on overflow set
    /// SF:  set on false
    /// ST:  set on true
    /// 
    /// Scc &lt;ea&gt;
    /// Size: Byte
    /// </summary>
    internal class OpcodeScc : BaseOpcode
    {
        public OpcodeScc()
            : base(IsMatch, ToAsm, header => SizeWithArg(header, 2))
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xf0c0) != 0x50c0)
                return false;

            if (((header >> 3) & 0x7) == 0x1) // would be DBcc
                return false;

            return true;
        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var arg = ParseArg(header, 10, dataReader, 1, addresses, AddressingModes.Default);
            var condition = (Condition)((header >> 8) & 0xf);

            return KeyValuePair.Create($"S{condition} {arg}", addresses);
        }
    }
}
