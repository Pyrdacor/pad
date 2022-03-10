using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// CMPM - Compare memory with memory
    /// 
    /// CMPM (Ay)+,(Ax)+
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeCmpm : BaseOpcode
    {
        public OpcodeCmpm()
            : base(IsMatch, ToAsm, _ => 2)
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xf100) != 0xb100)
                return false;

            if ((header & 0xc0) == 0xc0) // would be CPMA
                return false;

            return (header & 0x38) == 0x08;
        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            var dst = (header >> 9) & 0x7;
            var src = header & 0x7;

            string type = ((header >> 6) & 0x3) switch
            {
                0 => "B",
                1 => "W",
                2 => "L",
                _ => throw new InvalidDataException("Invalid CMPM instruction.")
            };

            return $"CMPM.{type} ({Global.AddressRegisterName(src)})+,({Global.AddressRegisterName(dst)})+";
        }
    }
}
