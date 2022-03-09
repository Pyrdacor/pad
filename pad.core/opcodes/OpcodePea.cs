using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// PEA - Push effective address
    /// 
    /// PEA &lt;ea&gt;
    /// Size: Long
    /// </summary>
    internal class OpcodePea : BaseOpcode
    {
        public OpcodePea()
            : base(0xffc0, 0x4840, ToAsm)
        {

        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new List<uint>();
            var arg = ParseArg(header, 10, dataReader, 1, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"PEA {arg}", addresses);
        }
    }
}
