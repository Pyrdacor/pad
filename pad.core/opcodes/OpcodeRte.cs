using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// RTE - Return from exception
    /// 
    /// RTE
    /// </summary>
    internal class OpcodeRte : ISimple16BitOpcode
    {
        public string ConvertToAsm(IDataReader reader)
        {
            reader.Position += 2;

            return "RTE";
        }
    }
}
