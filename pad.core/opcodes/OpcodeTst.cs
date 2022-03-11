using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// TST - Test an operand
    /// 
    /// TST &lt;ea&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeTst : BaseOpcode
    {
        public OpcodeTst()
            : base(IsMatch, ToAsm, header => SizeWithArg(header, 0))
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xff00) != 0x4a00)
                return false;

            if ((header & 0x00c0) == 0x00c0) // would be TAS
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
                _ => throw new InvalidDataException("Invalid NEGX instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, 0, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"TST.{suffix} {arg}", addresses);
        }
    }
}
