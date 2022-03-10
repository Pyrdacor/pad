using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// SUBI - Subtract immediate.
    /// 
    /// SUBI #&lt;data&gt;,&lt;ea&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeSubi : BaseOpcode
    {
        public OpcodeSubi()
            : base(0xff00, 0x0400, ToAsm, header => Math.Max(2, 2 * ((header >> 6) & 0x3)) + SizeWithArg(header, 0))
        {

        }

        static KeyValuePair<string, Dictionary<string, uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, uint>();
            Tuple<int, string, string, string> info = ((header >> 6) & 0x3) switch
            {
                0 => Tuple.Create(1, "B", "B", $"{dataReader.ReadWord() & 0xff:x2}"),
                1 => Tuple.Create(2, "W", "W", $"{dataReader.ReadWord():x4}"),
                2 => Tuple.Create(4, "L", "", $"{dataReader.ReadDword():x8}"),
                _ => throw new InvalidDataException("Invalid SUBI instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, AddressingModes.Default, info.Item3);

            return KeyValuePair.Create($"SUBI.{info.Item2} #{info.Item4},{arg}", addresses);
        }
    }
}
