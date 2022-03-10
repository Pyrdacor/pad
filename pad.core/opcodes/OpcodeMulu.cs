using pad.core.extensions;
using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// MULU - Unsigned multiply
    /// 
    /// MULU &lt;ea&gt;,Dn
    /// Size: Source Word, Destination Long
    /// </summary>
    internal class OpcodeMulu : BaseOpcode
    {
        public OpcodeMulu()
            : base(0xf1c0, 0xc0c0, ToAsm)
        {

        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new List<uint>();
            var reg = (header >> 9) & 0x7;
            var arg = ParseArg(header, 10, dataReader, 2, addresses, AddressingModes.All.Exclude(AddressingModes.AddressRegister), "W");

            return KeyValuePair.Create($"MULU {arg},D{reg}", addresses);
        }
    }
}
