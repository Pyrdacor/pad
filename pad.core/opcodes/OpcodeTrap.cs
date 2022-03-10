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
            : base(0xfff0, 0x4e40, ToAsm, _ => 2)
        {

        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            return $"TRAP #{header & 0xf}";
        }
    }
}
