using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// NEG - Negate
    /// 
    /// NEG &lt;ea&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeNeg : BaseOpcode
    {
        public OpcodeNeg()
            : base(IsMatch, ToAsm, header => SizeWithArg(header, 0))
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xff00) != 0x4400)
                return false;

            if ((header & 0x00c0) == 0x0030) // would be MOVE to CCR
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
                _ => throw new InvalidDataException("Invalid NEG instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, 0, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"NEG.{suffix} {arg}", addresses);
        }
    }
}
