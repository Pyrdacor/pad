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
            : base(IsMatch, ToAsm, Size)
        {

        }

        protected static int Size(ushort header)
        {
            int immediateBytes = (header >> 12) switch
            {
                1 => 2,
                2 => 4,
                3 => 2,
                _ => throw new InvalidDataException("Invalid MOVE instruction data.")
            };

            var headBits1 = (header >> 6) & 0x7;
            var regBits1 = (header >> 9) & 0x7;
            var headBits2 = (header >> 3) & 0x7;
            var regBits2 = header & 0x7;

            int size = 2;

            AddArgSize(headBits1, regBits1);
            AddArgSize(headBits2, regBits2);

            return size;

            void AddArgSize(int headBits, int regBits)
            {
                if (headBits == 5 | headBits == 6)
                    size += 2;
                else if (headBits == 7)
                    size += regBits switch
                    {
                        0 => 2,
                        1 => 4,
                        2 => 2,
                        3 => 2,
                        4 => immediateBytes,
                        _ => throw new InvalidDataException("Invalid address mode.")
                    };
            }
        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xc000) != 0)
                return false;

            if ((header & 0x01c0) == 0x0400) // would be MOVEA
                return false;

            return (header & 0x3000) != 0;
        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            Tuple<int, string> info = (header >> 12) switch
            {
                1 => Tuple.Create(1, "B"),
                2 => Tuple.Create(4, "L"),
                3 => Tuple.Create(2, "W"),
                _ => throw new InvalidDataException("Invalid MOVE instruction data.")
            };
            // NOTE: Parse source first, as additional parameters are read in the order <Source>, <Destination>!
            var src = ParseArg(header, 10, dataReader, info.Item1, addresses, AddressingModes.All);
            var dst = ParseArg(header, 4, dataReader, info.Item1, addresses, AddressingModes.Default | AddressingModes.Immediate, true);            

            return KeyValuePair.Create($"MOVE.{info.Item2} {src},{dst}", addresses);
        }
    }
}
