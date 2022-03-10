using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ANDI to CCR - AND immediate to condition code register.
    /// 
    /// ANDI #&lt;data&gt;,CCR
    /// Size: Byte
    /// </summary>
    internal class OpcodeAndiToCcr : ISimple16BitOpcode
    {
        public string ConvertToAsm(IDataReader reader)
        {
            reader.Position += 2;
            var arg = reader.ReadWord() & 0xff;

            return $"ANDI #{arg:x2},CCR";
        }

        public int Size => 4;
    }
}
