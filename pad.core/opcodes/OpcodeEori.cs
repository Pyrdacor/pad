using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// EORI - Exclusive OR immediate.
    /// 
    /// EORI #&lt;data&gt;,&lt;ea&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeEori : BaseOpcode
    {
        public OpcodeEori()
            : base(0xff00, 0x0a00, ToAsm)
        {
            // Note: We only match if the first byte is 10 here. This would normally
            // match EoriToCcr and EoriToSr as well but they are handled beforehand.
            // Eori does not allow immediate values as this would collide with the
            // other two and won't make any sense as you would XOR two immediate values.
        }

        static KeyValuePair<string, Dictionary<string, uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, uint>();
            Tuple<int, string, string, string> info = ((header >> 6) & 0x3) switch
            {
                0 => Tuple.Create(1, "B", "B", $"{dataReader.ReadByte():x2}"),
                1 => Tuple.Create(2, "W", "W", $"{dataReader.ReadWord():x4}"),
                2 => Tuple.Create(4, "L", "", $"{dataReader.ReadDword():x8}"),
                _ => throw new InvalidDataException("Invalid ORI instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, AddressingModes.Default, info.Item3);

            return KeyValuePair.Create($"EORI.{info.Item2} #{info.Item4},{arg}", addresses);
        }
    }
}
