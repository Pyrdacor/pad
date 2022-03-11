using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ANDI - AND immediate.
    /// 
    /// ANDI #&lt;data&gt;,&lt;ea&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeAndi : BaseOpcode
    {
        public OpcodeAndi()
            : base(0xff00, 0x0200, ToAsm, header => Math.Max(2, 2 * ((header >> 6) & 0x3)) + SizeWithArg(header, 0))
        {
            // Note: We only match if the first byte is 2 here. This would normally
            // match AndiToCcr and AndiToSr as well but they are handled beforehand.
            // Andi does not allow immediate values as this would collide with the
            // other two and won't make any sense as you would AND two immediate values.
        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            Tuple<int, string, string> info = ((header >> 6) & 0x3) switch
            {
                0 => Tuple.Create(1, "B", $"{dataReader.ReadWord() & 0xff:x2}"),
                1 => Tuple.Create(2, "W", $"{dataReader.ReadWord():x4}"),
                2 => Tuple.Create(4, "L", $"{dataReader.ReadDword():x8}"),
                _ => throw new InvalidDataException("Invalid ANDI instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"ANDI.{info.Item2} #${info.Item3},{arg}", addresses);
        }
    }
}
