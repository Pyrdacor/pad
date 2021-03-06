using pad.core.extensions;
using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// PEA - Push effective address
    /// 
    /// PEA &lt;ea&gt;
    /// Size: Long
    /// </summary>
    internal class OpcodePea : BaseOpcode
    {
        public OpcodePea()
            : base(0xffc0, 0x4840, ToAsm, header => SizeWithArg(header, 0))
        {

        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var addressingModes = AddressingModes.All.Exclude(AddressingModes.DataRegister, AddressingModes.AddressRegister,
                AddressingModes.AddressWithPost, AddressingModes.AddressWithPre, AddressingModes.Immediate);
            var arg = ParseArg(header, 10, dataReader, 1, addresses, addressingModes);

            return KeyValuePair.Create($"PEA {arg}", addresses);
        }
    }
}
