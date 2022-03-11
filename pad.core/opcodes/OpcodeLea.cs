using pad.core.extensions;
using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// LEA - Load effective address
    /// 
    /// LEA &lt;ea&gt;,An
    /// Size: Long
    /// </summary>
    internal class OpcodeLea : BaseOpcode
    {
        public OpcodeLea()
            : base(0xf1c0, 0x41c0, ToAsm, header => SizeWithArg(header, 0))
        {

        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var reg = Global.AddressRegisterName((header >> 9) & 0x7);
            var addressingModes = AddressingModes.All.Exclude(AddressingModes.DataRegister, AddressingModes.AddressRegister,
                AddressingModes.AddressWithPost, AddressingModes.AddressWithPre, AddressingModes.Immediate);
            var arg = ParseArg(header, 10, dataReader, 4, addresses, addressingModes);

            return KeyValuePair.Create($"LEA {arg},{reg}", addresses);
        }
    }
}
