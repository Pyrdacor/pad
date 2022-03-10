using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// EORI to CCR - Exclusive OR immediate to condition code register.
    /// 
    /// EORI #&lt;data&gt;,CCR
    /// Size: Byte
    /// </summary>
    internal class OpcodeEoriToCcr : ISimple16BitOpcode
    {
        public string ConvertToAsm(IDataReader reader)
        {
            reader.Position += 2;
            var arg = reader.ReadWord() & 0xff;

            return $"EORI #{arg:x2},CCR";
        }

        public int Size => 4;
    }
}
