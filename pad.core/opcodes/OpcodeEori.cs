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
            : base(0xff00, 0x0a00, ToAsm, header => Math.Max(2, 2 * ((header >> 6) & 0x3)) + SizeWithArg(header, 0))
        {
            // Note: We only match if the first byte is 10 here. This would normally
            // match EoriToCcr and EoriToSr as well but they are handled beforehand.
            // Eori does not allow immediate values as this would collide with the
            // other two and won't make any sense as you would XOR two immediate values.
        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            Tuple<string, string> info = ((header >> 6) & 0x3) switch
            {
                0 => Tuple.Create("B", $"{dataReader.ReadWord() & 0xff:x2}"),
                1 => Tuple.Create("W", $"{dataReader.ReadWord():x4}"),
                2 => Tuple.Create("L", $"{dataReader.ReadDword():x8}"),
                _ => throw new InvalidDataException("Invalid EORI instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, 0, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"EORI.{info.Item1} #${info.Item2},{arg}", addresses);
        }
    }
}
