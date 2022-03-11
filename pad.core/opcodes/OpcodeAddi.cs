using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ADDI - Add immediate.
    /// 
    /// ADDI #&lt;data&gt;,&lt;ea&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeAddi : BaseOpcode
    {
        public OpcodeAddi()
            : base(0xff00, 0x0600, ToAsm, header => Math.Max(2, 2 * ((header >> 6) & 0x3)) + SizeWithArg(header, 0))
        {

        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            Tuple<int, string, string> info = ((header >> 6) & 0x3) switch
            {
                0 => Tuple.Create(1, "B", $"{dataReader.ReadWord() & 0xff:x2}"),
                1 => Tuple.Create(2, "W", $"{dataReader.ReadWord():x4}"),
                2 => Tuple.Create(4, "L", $"{dataReader.ReadDword():x8}"),
                _ => throw new InvalidDataException("Invalid ADDI instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"ADDI.{info.Item2} #${info.Item3},{arg}", addresses);
        }
    }
}
