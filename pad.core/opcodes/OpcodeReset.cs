using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// RESET - Reset external devices
    /// 
    /// RESET
    /// </summary>
    internal class OpcodeReset : ISimple16BitOpcode
    {
        public string ConvertToAsm(IDataReader reader)
        {
            reader.Position += 2;

            return "RESET";
        }

        public int Size => 2;
    }
}
