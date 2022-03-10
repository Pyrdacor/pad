﻿using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// JSR - Jump to subroutine
    /// 
    /// JSR &lt;ea&gt;
    /// </summary>
    internal class OpcodeJsr : BaseJumpOpcode
    {        
        public OpcodeJsr()
            : base(0xffc0, 0x4e80, ToAsm)
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

            return KeyValuePair.Create($"JSR {arg}", addresses);
        }
    }
}
