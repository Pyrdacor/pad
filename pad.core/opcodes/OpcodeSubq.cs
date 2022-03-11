using pad.core.extensions;
using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// SUBQ - Subtract quick
    /// 
    /// SUBQ #&lt;data&gt;,&lt;ea&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeSubq : BaseOpcode
    {
        public OpcodeSubq()
            : base(IsMatch, ToAsm, header => SizeWithArg(header, 0))
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xf100) != 0x5100)
                return false;

            if (((header >> 6) & 0x3) == 0x3) // would be Scc or DBcc
                return false;

            return true;
        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            string suffix = ((header >> 6) & 0x3) switch
            {
                0 => "B",
                1 => "W",
                2 => "L",
                _ => throw new InvalidDataException("Invalid SUBQ instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, 0, addresses,
                AddressingModes.All.Exclude(AddressingModes.PCWithDisplacement, AddressingModes.PCWithIndex, AddressingModes.Immediate));
            var value = 1 + ((header >> 9) & 0x07);

            return KeyValuePair.Create($"SUBQ.{suffix} #{value},{arg}", addresses);
        }
    }
}
