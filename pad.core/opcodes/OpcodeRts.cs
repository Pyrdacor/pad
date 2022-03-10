using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// RTS - Return from subroutine
    /// 
    /// RTS
    /// </summary>
    internal class OpcodeRts : ISimple16BitOpcode
    {
        public string ConvertToAsm(IDataReader reader)
        {
            reader.Position += 2;

            return "RTS";
        }

        public int Size => 2;
    }
}
