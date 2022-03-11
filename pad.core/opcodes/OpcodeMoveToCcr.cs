using pad.core.extensions;
using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// MOVE to CCR - Copy data to condition code register from source
    /// 
    /// MOVE &lt;ea&gt;,CCR
    /// Size: Word
    /// </summary>
    internal class OpcodeMoveToCcr : BaseOpcode
    {
        public OpcodeMoveToCcr()
            : base(0xffc0, 0x44c0, ToAsm, header => SizeWithArg(header, 2))
        {

        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var arg = ParseArg(header, 10, dataReader, 2, addresses, AddressingModes.All.Exclude(AddressingModes.AddressRegister));

            return KeyValuePair.Create($"MOVE.W {arg},CCR", addresses);
        }
    }
}
