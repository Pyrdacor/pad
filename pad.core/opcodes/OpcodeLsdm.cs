using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// LSL, LSR - Logical shift left/right (memory version)
    /// 
    /// LSL &lt;ea&gt;
    /// LSR &lt;ea&gt;
    /// Size: Word
    /// </summary>
    internal class OpcodeLsdm : BaseOpcode
    {
        public OpcodeLsdm()
            : base(0xfec0, 0xe2c0, ToAsm, header => SizeWithArg(header, 2))
        {

        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            string dir = (header & 0x0100) == 0 ? "R" : "L";
            var addresses = new Dictionary<string, Reference>();
            var arg = ParseArg(header, 10, dataReader, 2, addresses, AddressingModes.Memory);

            return KeyValuePair.Create($"LS{dir}.W {arg}", addresses);
        }
    }
}
