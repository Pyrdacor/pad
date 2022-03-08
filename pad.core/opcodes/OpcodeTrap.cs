using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// TRAP - Trap
    /// 
    /// TRAP #&lt;vector&gt;
    /// </summary>
    internal class OpcodeTrap : BaseOpcode
    {
        public OpcodeTrap()
            : base(0xfff0, 0x4e40, ToAsm)
        {

        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            return $"UNLK #{header & 0x4}";
        }
    }
}
