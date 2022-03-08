using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// MOVE USP - Copy data to or from USP (user stack pointer)
    /// 
    /// MOVE USP,An
    /// MOVE An,USP
    /// </summary>
    internal class OpcodeMoveUsp : BaseOpcode
    {
        public OpcodeMoveUsp()
            : base(Match, ToAsm)
        {

        }

        static bool Match(ushort header)
        {
            header &= 0xfff8;

            return header == 0x4e60 || header == 0x4e68;
        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            int reg = header & 0x3;
            header &= 0xfff8;

            return header switch
            {
                0x4e60 => $"MOVE A{reg},USP",
                0x4e68 => $"MOVE USP,A{reg}",
                _ => throw new InvalidDataException("Invalid MOVE USP instruction.")
            };
        }
    }
}
