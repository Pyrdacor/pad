using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ASL, ASR - Arithmetic shift left/right (memory version)
    /// 
    /// ASL &lt;ea&gt;
    /// ASR &lt;ea&gt;
    /// Size: Word
    /// </summary>
    internal class OpcodeAsdm : BaseOpcode
    {
        public OpcodeAsdm()
            : base(0xfec0, 0xe0c0, ToAsm, header => SizeWithArg(header, 2))
        {

        }

        static KeyValuePair<string, Dictionary<string, uint>> ToAsm(ushort header, IDataReader dataReader)
        {
            string dir = (header & 0x0100) == 0 ? "R" : "L";
            var addresses = new Dictionary<string, uint>();
            var arg = ParseArg(header, 10, dataReader, 2, addresses, AddressingModes.Memory, "W");

            return KeyValuePair.Create($"AS{dir}.W {arg}", addresses);
        }
    }
}
