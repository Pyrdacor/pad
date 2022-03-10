using pad.core.extensions;
using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// OR - OR logical
    /// 
    /// OR &lt;ea&gt;,Dn
    /// OR Dn,&lt;ea&gt;
    /// Size: Byte, Word, Long
    /// </summary>
    internal class OpcodeOr : BaseOpcode
    {
        public OpcodeOr()
            : base(IsMatch, ToAsm)
        {

        }

        static bool IsMatch(ushort header)
        {
            if ((header >> 12) != 0x8)
                return false;

            if ((header & 0xc0) == 0xc0) // would be DIVU or DIVS
                return false;

            var tempHeader = header & 0xf1f8;

            if (tempHeader == 0x8100 || tempHeader == 0x8108) // would be SCBD
                return false;

            return true;
        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new List<uint>();
            var reg = (header >> 9) & 0x7;
            bool toDataRegister = (header & 0x0100) == 0;
            var addressingModes = toDataRegister
                ? AddressingModes.All.Exclude(AddressingModes.AddressRegister)
                : AddressingModes.Default;
            Tuple<int, string, string> info = ((header >> 6) & 0x3) switch
            {
                0 => Tuple.Create(1, "B", "B"),
                1 => Tuple.Create(2, "W", "W"),
                2 => Tuple.Create(4, "L", ""),
                _ => throw new InvalidDataException("Invalid OR instruction.")
            };
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, addressingModes, info.Item3);

            return toDataRegister
                ? KeyValuePair.Create($"OR.{info.Item2} {arg},D{reg}{info.Item3}", addresses)
                : KeyValuePair.Create($"OR.{info.Item2} D{reg}{info.Item3},{arg}", addresses);
        }
    }
}
