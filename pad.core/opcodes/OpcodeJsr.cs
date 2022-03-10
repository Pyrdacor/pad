using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// JSR - Jump to subroutine
    /// 
    /// JSR &lt;ea&gt;
    /// </summary>
    internal class OpcodeJsr : BaseOpcode, IJumpOpcode
    {        
        public OpcodeJsr()
            : base(0xffc0, 0x4e80, ToAsm)
        {

        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new List<uint>();
            var arg = ParseArg(header, 10, dataReader, 1, addresses,
                AddressingModes.Address | AddressingModes.AddressWithDisplacement |
                AddressingModes.AddressWithIndex | AddressingModes.AbsoluteShort |
                AddressingModes.AbsoluteLong | AddressingModes.PCWithDisplacement |
                AddressingModes.PCWithIndex);

            return KeyValuePair.Create($"JSR {arg}", addresses);
        }
    }
}
