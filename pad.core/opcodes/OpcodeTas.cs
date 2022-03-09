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
            : base(0xffc0, 0x4ac0, ToAsm)
        {

        }

        static KeyValuePair<string, List<uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new List<uint>();
            var arg = ParseArg(header, 10, dataReader, 1, addresses, AddressingModes.Default, "B");

            return KeyValuePair.Create($"TAS {arg}", addresses);
        }
    }
}
