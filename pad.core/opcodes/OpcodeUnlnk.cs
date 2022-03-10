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
            : base(0xfff8, 0x4e58, ToAsm, _ => 2)
        {

        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            return $"UNLK {Global.AddressRegisterName(header & 0x7)}";
        }
    }
}
