using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// EORI to SR - Exclusive OR immediate to status register.
    /// 
    /// EORI #&lt;data&gt;,SR
    /// Size: Word
    /// </summary>
    internal class OpcodeEoriToSr : ISimple16BitOpcode
    {
        public string ConvertToAsm(IDataReader reader)
        {
            reader.Position += 2;
            var arg = reader.ReadWord();

            return $"EORI #${arg:x4},SR";
        }

        public int Size => 4;
    }
}
