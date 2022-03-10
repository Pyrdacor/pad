using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// EXT - Sign-extend a data register
    /// 
    /// EXT.W Dn
    /// EXT.L Dn
    /// </summary>
    internal class OpcodeExt : BaseOpcode
    {
        public OpcodeExt()
            : base(Match, ToAsm, _ => 2)
        {

        }

        static bool Match(ushort header)
        {
            header &= 0xfff8;

            return header == 0x4880 || header == 0x48c0;
        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            int reg = header & 0x7;
            header &= 0xfff8;

            return header switch
            {
                0x4880 => $"EXT.W D{reg}",
                0x48c0 => $"EXT.L D{reg}",
                _ => throw new InvalidDataException("Invalid EXT instruction.")
            };
        }
    }
}
