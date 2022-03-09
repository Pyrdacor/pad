using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// MOVE - Copy data from source to destination
    /// 
    /// MOVE &lt;ea&gt;,&lt;e&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeMove : BaseOpcode
    {
        public OpcodeMove()
            : base(IsMatch, ToAsm)
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xc000) != 0)
                return false;

            if ((header & 0x01c0) == 0x0100) // would be MOVEA
                return false;

            return (header & 0x3000) != 0;
        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new List<uint>();
            Tuple<int, string, string> info = (header >> 12) switch
            {
                1 => Tuple.Create(1, "B", "B"),
                2 => Tuple.Create(4, "L", ""),
                3 => Tuple.Create(2, "W", "W"),
                _ => throw new InvalidDataException("Invalid MOVE instruction data.")
            };
            var dst = ParseArg(header, 4, dataReader, info.Item1, addresses, AddressingModes.Default | AddressingModes.Immediate, info.Item3, true, true);
            var src = ParseArg(header, 10, dataReader, info.Item1, addresses, AddressingModes.All, info.Item3);

            return KeyValuePair.Create($"MOVE.{info.Item2} {src},{dst}", addresses);
        }
    }
}
