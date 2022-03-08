using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ANDI to SR - AND immediate to status register.
    /// 
    /// ANDI #&lt;data&gt;,SR
    /// Size: Word
    /// </summary>
    internal class OpcodeAndiToSr : ISimple16BitOpcode
    {
        public string ConvertToAsm(IDataReader reader)
        {
            reader.Position += 2;
            var arg = reader.ReadWord();

            return $"ANDI #{arg:x4},SR";
        }
    }
}
