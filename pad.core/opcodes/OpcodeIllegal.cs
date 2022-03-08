using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ILLEGAL - Illegal instruction
    /// 
    /// ILLEGAL
    /// </summary>
    internal class OpcodeIllegal : ISimple16BitOpcode
    {
        public string ConvertToAsm(IDataReader reader)
        {
            reader.Position += 2;

            return "ILLEGAL";
        }
    }
}
