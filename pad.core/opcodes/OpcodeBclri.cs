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
            : base(0xffc0, 0x0880, ToAsm)
        {

        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new List<uint>();
            var bitIndex = dataReader.ReadByte();
            var arg = ParseArg(header, 10, dataReader, 0, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"BCLR.B #{bitIndex},{arg}", addresses);
        }
    }
}
