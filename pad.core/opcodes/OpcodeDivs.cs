using pad.core.extensions;
using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// DIVS - Signed divide
    /// 
    /// DIVS &lt;ea&gt;,Dn
    /// Size: Source Word, Destination Long
    /// </summary>
    internal class OpcodeDivs : BaseOpcode
    {
        public OpcodeDivs()
            : base(0xf1c0, 0x81c0, ToAsm)
        {

        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new List<uint>();
            var reg = (header >> 9) & 0x7;
            var arg = ParseArg(header, 10, dataReader, 2, addresses, AddressingModes.All.Exclude(AddressingModes.AddressRegister), "W");

            return KeyValuePair.Create($"DIVS {arg},D{reg}", addresses);
        }
    }
}
