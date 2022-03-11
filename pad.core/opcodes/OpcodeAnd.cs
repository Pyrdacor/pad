using pad.core.extensions;
using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// AND - AND logical
    /// 
    /// AND &lt;ea&gt;,Dn
    /// AND Dn,&lt;ea&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeAnd : BaseOpcode
    {
        public OpcodeAnd()
            : base(IsMatch, ToAsm, header => SizeWithArg(header, 2 * ((header >> 6) & 0x3)))
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header >> 12) != 0xc)
                return false;

            if ((header & 0xc0) == 0xc0) // would be MULU or MULS
                return false;

            var tempHeader = header & 0xf1f8;

            if (tempHeader == 0xc100 || tempHeader == 0xc108) // would be ACBD
                return false;

            if ((header & 0x0100) != 0) // possibly EXG
            {
                var opmode = (header >> 3) & 0x1f;

                if (opmode == 0x08 || opmode == 0x09 || opmode == 0x11) // would be EXG
                    return false;
            }

            return true;
        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var reg = (header >> 9) & 0x7;
            bool toDataRegister = (header & 0x0100) == 0;
            var addressingModes = toDataRegister
                ? AddressingModes.All.Exclude(AddressingModes.AddressRegister)
                : AddressingModes.Default;
            Tuple<int, string> info = ((header >> 6) & 0x3) switch
            {
                0 => Tuple.Create(1, "B"),
                1 => Tuple.Create(2, "W"),
                2 => Tuple.Create(4, "L"),
                _ => throw new InvalidDataException("Invalid AND instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, addressingModes);

            return toDataRegister
                ? KeyValuePair.Create($"AND.{info.Item2} {arg},D{reg}", addresses)
                : KeyValuePair.Create($"AND.{info.Item2} D{reg},{arg}", addresses);
        }
    }
}
