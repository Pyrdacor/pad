using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// SWAP - Swap register halves
    /// 
    /// SWAP Dn
    /// </summary>
    internal class OpcodeSwap : BaseOpcode
    {
        public OpcodeSwap()
            : base(0xfff8, 0x4840, ToAsm, _ => 2)
        {

        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            return $"SWAP D{header & 0x7}";
        }
    }
}
