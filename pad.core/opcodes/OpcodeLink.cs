using pad.core.extensions;
using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// LINK - Link and allocate
    /// 
    /// LINK An,#&lt;displacement&gt;
    /// Size: Word
    /// </summary>
    internal class OpcodeLink : BaseOpcode
    {
        public OpcodeLink()
            : base(0xfff8, 0x4e50, ToAsm)
        {

        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            var displacement = dataReader.ReadDisplacement();

            return $"LINK {Global.AddressRegisterName(header & 0x7)},#{displacement}";
        }
    }
}
