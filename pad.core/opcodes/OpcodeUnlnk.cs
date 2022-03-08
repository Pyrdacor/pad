using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// UNLK - Unlink
    /// 
    /// UNLK An
    /// </summary>
    internal class OpcodeUnlnk : BaseOpcode
    {
        public OpcodeUnlnk()
            : base(0xfff8, 0x4e58, ToAsm)
        {

        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            return $"UNLK A{header & 0x3}";
        }
    }
}
