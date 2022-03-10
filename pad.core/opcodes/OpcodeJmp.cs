using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// JMP - Jump (unconditionally)
    /// 
    /// JMP &lt;ea&gt;
    /// </summary>
    internal class OpcodeJmp : BaseJumpOpcode
    {
        public OpcodeJmp()
            : base(0xffc0, 0x4ec0, ToAsm, header => SizeWithArg(header, 0))
        {

        }

        static KeyValuePair<string, Dictionary<string, uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, uint>();
            var arg = ParseArg(header, 10, dataReader, 1, addresses,
                AddressingModes.Address | AddressingModes.AddressWithDisplacement |
                AddressingModes.AddressWithIndex | AddressingModes.AbsoluteShort |
                AddressingModes.AbsoluteLong | AddressingModes.PCWithDisplacement |
                AddressingModes.PCWithIndex, "", false, true);

            return KeyValuePair.Create($"JMP {arg}", addresses);
        }
    }
}
