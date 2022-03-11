using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ORI to SR - Inclusive OR immediate to status register.
    /// 
    /// ORI #&lt;data&gt;,SR
    /// Size: Word
    /// </summary>
    internal class OpcodeOriToSr : ISimple16BitOpcode
    {
        public string ConvertToAsm(IDataReader reader)
        {
            reader.Position += 2;
            var arg = reader.ReadWord();

            return $"ORI #${arg:x4},SR";
        }

        public int Size => 4;
    }
}
