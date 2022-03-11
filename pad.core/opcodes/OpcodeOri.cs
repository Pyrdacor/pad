using pad.core.extensions;
using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ORI - Inclusive OR immediate.
    /// 
    /// ORI #&lt;data&gt;,&lt;ea&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeOri : BaseOpcode
    {
        public OpcodeOri()
            : base(0xff00, 0x0000, ToAsm, header => Math.Max(2, 2 * ((header >> 6) & 0x3)) + SizeWithArg(header, 0))
        {
            // Note: We only match if the first byte is 0 here. This would normally
            // match OriToCcr and OriToSr as well but they are handled beforehand.
            // Ori does not allow immediate values as this would collide with the
            // other two and won't make any sense as you would OR two immediate values.
        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            Tuple<string, string> info = ((header >> 6) & 0x3) switch
            {
                0 => Tuple.Create("B", $"{dataReader.ReadWord() & 0xff:x2}"),
                1 => Tuple.Create("W", $"{dataReader.ReadWord():x4}"),
                2 => Tuple.Create("L", $"{dataReader.ReadDword():x8}"),
                _ => throw new InvalidDataException("Invalid ORI instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, 0, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"ORI.{info.Item1} #${info.Item2},{arg}", addresses);
        }
    }
}
