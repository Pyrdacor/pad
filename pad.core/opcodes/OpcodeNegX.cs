﻿using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// NEGX - Negate with extend
    /// 
    /// NEGX &lt;ea&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeNegX : BaseOpcode
    {
        public OpcodeNegX()
            : base(IsMatch, ToAsm)
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xff00) != 0x4000)
                return false;

            if ((header & 0x00c0) == 0x0030) // would be MOVE from SR
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
                _ => throw new InvalidDataException("Invalid NEGX instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, AddressingModes.Default, info.Item3);

            return KeyValuePair.Create($"NEGX.{info.Item2} {arg}", addresses);
        }
    }
}
