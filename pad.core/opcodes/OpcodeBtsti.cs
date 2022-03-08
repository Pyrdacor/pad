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
            : base(0xffc0, 0x0800, ToAsm)
        {

        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new List<uint>();
            var bitIndex = dataReader.ReadByte();
            var arg = ParseArg(header, 10, dataReader, 0, addresses, false);

            return KeyValuePair.Create($"BTST #{bitIndex},{arg}", addresses);
        }
    }
}
