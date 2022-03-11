using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ROL, ROR - Rotate left/right (data register versions)
    /// 
    /// ROL Dx,Dy
    /// ROR Dx, Dy
    /// ROL #&lt;data&gt;,Dy
    /// ROR #&lt;data&gt;,Dy
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeRod : BaseOpcode
    {
        public OpcodeRod()
            : base(IsMatch, ToAsm, _ => 2)
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header & 0xf018) != 0xe018)
                return false;

            if ((header & 0x00c0) == 0x00c0) // would be the memory version of ROd
                return false;

            return true;
        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            string dir = (header & 0x0100) == 0 ? "R" : "L";
            var addresses = new Dictionary<string, Reference>();
            var amount = (header >> 9) & 0x7;
            string suffix = ((header >> 6) & 0x3) switch
            {
                0 => "B",
                1 => "W",
                2 => "L",
                _ => throw new InvalidDataException("Invalid ROd instruction.")
            };
            string amountStr = (header & 0x20) == 0
                ? $"#${amount:x2}"
                : $"D{amount}";
            var reg = header & 0x7;

            return KeyValuePair.Create($"RO{dir}.{suffix} {amountStr},D{reg}", addresses);
        }
    }
}
