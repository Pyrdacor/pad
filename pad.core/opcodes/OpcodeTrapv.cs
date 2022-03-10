using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// TRAPV - Trap on overflow
    /// 
    /// TRAPV
    /// </summary>
    internal class OpcodeTrapv : ISimple16BitOpcode
    {
        public string ConvertToAsm(IDataReader reader)
        {
            reader.Position += 2;

            return "TRAPV";
        }

        public int Size => 2;
    }
}
