﻿using pad.core.extensions;
using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ADDQ - Add quick
    /// 
    /// ADDQ #&lt;data&gt;,&lt;ea&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeAddq : BaseOpcode
    {
        public OpcodeAddq()
            : base(IsMatch, ToAsm)
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xf100) != 0x5000)
                return false;

            if (((header >> 6) & 0x3) == 0x3) // would be Scc or DBcc
                return false;

            return true;
        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new List<uint>();
            Tuple<int, string, string> info = ((header >> 6) & 0x3) switch
            {
                0 => Tuple.Create(1, "B", "B"),
                1 => Tuple.Create(2, "W", "W"),
                2 => Tuple.Create(4, "L", ""),
                _ => throw new InvalidDataException("Invalid ADDQ instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses,
                AddressingModes.All.Exclude(AddressingModes.PCWithDisplacement, AddressingModes.PCWithIndex, AddressingModes.Immediate),
                info.Item3);
            var value = 1 + ((header >> 9) & 0x07);

            return KeyValuePair.Create($"ADDQ.{info.Item2} #{value},{arg}", addresses);
        }
    }
}