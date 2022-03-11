using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// BCLR - Test a bit and clear
    /// 
    /// BCLR #&lt;data&gt;,&lt;ea&gt;
    /// Size: Byte
    /// </summary>
    internal class OpcodeBclri : BaseOpcode
    {
        public OpcodeBclri()
            : base(0xffc0, 0x0880, ToAsm, header => 2 + SizeWithArg(header, 0))
        {

        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var bitIndex = dataReader.ReadWord() & 0xff;
            var arg = ParseArg(header, 10, dataReader, 0, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"BCLR.B #{bitIndex},{arg}", addresses);
        }
    }
}
