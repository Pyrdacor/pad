using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// EOR - Exclusive-OR logical
    /// 
    /// EOR Dn,&lt;ea&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeEor : BaseOpcode
    {
        public OpcodeEor()
            : base(IsMatch, ToAsm, header => SizeWithArg(header, 0))
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xf100) != 0xb100)
                return false;

            if ((header & 0xc0) == 0xc0) // would be CPMA
                return false;

            if ((header & 0x38) == 0x08) // would be CMPM
                return false;

            return true;
        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var reg = (header >> 9) & 0x7;

            string suffix = ((header >> 6) & 0x3) switch
            {
                0 => "B",
                1 => "W",
                2 => "L",
                _ => throw new InvalidDataException("Invalid EOR instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, 0, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"EOR.{suffix} D{reg},{arg}", addresses);
        }
    }
}
