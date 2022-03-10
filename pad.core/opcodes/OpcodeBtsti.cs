using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// BTST - Test a bit
    /// 
    /// BTST #&lt;data&gt;,&lt;ea&gt;
    /// Size: Byte
    /// </summary>
    internal class OpcodeBtsti : BaseOpcode
    {
        public OpcodeBtsti()
            : base(0xffc0, 0x0800, ToAsm, header => 2 + SizeWithArg(header, 0))
        {

        }

        static KeyValuePair<string, Dictionary<string, uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, uint>();
            var bitIndex = dataReader.ReadWord() & 0xff;
            var arg = ParseArg(header, 10, dataReader, 0, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"BTST.B #{bitIndex},{arg}", addresses);
        }
    }
}
