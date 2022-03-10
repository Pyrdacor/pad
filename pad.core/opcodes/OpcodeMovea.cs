using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// MOVEA - Move address
    /// 
    /// MOVEA &lt;ea&gt;,An
    /// Size: Word, Long
    /// </summary>
    internal class OpcodeMovea : BaseOpcode
    {
        public OpcodeMovea()
            : base(0xe1c0, 0x2040, ToAsm)
        {

        }

        static KeyValuePair<string, Dictionary<string, uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addrRegister = Global.AddressRegisterName((header >> 9) & 0x7);
            var addresses = new Dictionary<string, uint>();
            Tuple<int, string, string> info = (header & 0x1000) == 0
                ? Tuple.Create(4, "L", "")
                : Tuple.Create(2, "W", "W");
            var arg = ParseArg(header, 10, dataReader, info.Item1, addresses, AddressingModes.All, info.Item3);

            return KeyValuePair.Create($"MOVEA.{info.Item2} {arg},{addrRegister}", addresses);
        }
    }
}
