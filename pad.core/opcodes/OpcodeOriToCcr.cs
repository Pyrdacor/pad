using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// ORI to CCR - Inclusive OR immediate to condition code register.
    /// 
    /// ORI #&lt;data&gt;,CCR
    /// Size: Byte
    /// </summary>
    internal class OpcodeOriToCcr : ISimple16BitOpcode
    {
        public string ConvertToAsm(IDataReader reader)
        {
            reader.Position += 2;
            var arg = reader.ReadByte();

            return $"ORI #{arg:x2},CCR";
        }
    }
}
