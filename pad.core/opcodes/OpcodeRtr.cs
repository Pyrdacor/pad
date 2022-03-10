using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// RTR - Return and restore condition codes
    /// 
    /// RTR
    /// </summary>
    internal class OpcodeRtr : ISimple16BitOpcode
    {
        public string ConvertToAsm(IDataReader reader)
        {
            reader.Position += 2;

            return "RTR";
        }

        public int Size => 2;
    }
}
