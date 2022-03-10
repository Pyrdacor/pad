using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// NOP - No operation
    /// 
    /// NOP
    /// </summary>
    internal class OpcodeNop : ISimple16BitOpcode
    {
        public string ConvertToAsm(IDataReader reader)
        {
            reader.Position += 2;

            return "NOP";
        }

        public int Size => 2;
    }
}
