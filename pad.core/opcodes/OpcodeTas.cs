using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// TAS - Test and set an operand
    /// 
    /// TAS &lt;ea&gt;
    /// Size: Byte
    /// </summary>
    internal class OpcodeTas : BaseOpcode
    {
        public OpcodeTas()
            : base(0xffc0, 0x4ac0, ToAsm, header => SizeWithArg(header, 2))
        {

        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var arg = ParseArg(header, 10, dataReader, 1, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"TAS {arg}", addresses);
        }
    }
}
