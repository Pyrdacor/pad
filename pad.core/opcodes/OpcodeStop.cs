using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// STOP - Load status register and stop
    /// 
    /// STOP #&lt;data&gt;
    /// Size: Word
    /// </summary>
    internal class OpcodeStop : ISimple16BitOpcode
    {
        public string ConvertToAsm(IDataReader reader)
        {
            reader.Position += 2;
            var arg = reader.ReadWord();

            return $"STOP #{arg:x4}";
        }
    }
}
