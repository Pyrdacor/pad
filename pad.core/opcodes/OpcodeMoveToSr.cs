using pad.core.extensions;
using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// MOVE to SR - Copy data to status register from source
    /// 
    /// MOVE &lt;ea&gt;,SR
    /// Size: Word
    /// </summary>
    internal class OpcodeMoveToSr : BaseOpcode
    {
        public OpcodeMoveToSr()
            : base(0xffc0, 0x46c0, ToAsm, header => SizeWithArg(header, 2))
        {

        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var arg = ParseArg(header, 10, dataReader, 2, addresses, AddressingModes.All.Exclude(AddressingModes.AddressRegister));

            return KeyValuePair.Create($"MOVE.W {arg},SR", addresses);
        }
    }
}
