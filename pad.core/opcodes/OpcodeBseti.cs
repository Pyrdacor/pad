﻿using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// BSET - Test a bit and set
    /// 
    /// BSET #&lt;data&gt;,&lt;ea&gt;
    /// Size: Byte
    /// </summary>
    internal class OpcodeBseti : BaseOpcode
    {
        public OpcodeBseti()
            : base(0xffc0, 0x08c0, ToAsm)
        {

        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new List<uint>();
            var bitIndex = dataReader.ReadByte();
            var arg = ParseArg(header, 10, dataReader, 0, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"BSET.B #{bitIndex},{arg}", addresses);
        }
    }
}
