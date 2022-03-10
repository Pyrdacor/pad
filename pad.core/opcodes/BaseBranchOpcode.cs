using pad.core.interfaces;
using pad.core.util;

namespace pad.core.opcodes
{
    internal class BaseBranchOpcode : BaseOpcode, IBranchOpcode
    {
        static int GetDisplacement(uint data)
        {
            int d = (int)(data >> 16) & 0xff;

            if (d == 0)
                d = WordConverter.AsSigned((ushort)(data & 0xffff));
            else
                d = ByteConverter.AsSigned((byte)d);

            return d;
        }

        protected BaseBranchOpcode(Func<ushort, bool> matcher, Func<ushort, IDataReader, string> asmProvider, bool unconditional)
            : base(matcher, asmProvider)
        {
            Unconditional = unconditional;
        }

        protected BaseBranchOpcode(ushort mask, ushort value, Func<ushort, IDataReader, string> asmProvider, bool unconditional)
            : base(mask, value, asmProvider)
        {
            Unconditional = unconditional;
        }

        public int Displacement { get; private set; }
        public bool Unconditional { get; }

        public override bool TryMatch(IDataReader reader, out string asm, out Dictionary<string, uint> references)
        {
            if (base.TryMatch(reader, out asm, out references))
            {
                Displacement = GetDisplacement(reader.Size - reader.Position < 4 ? reader.PeekWord() : reader.PeekDword());
                return true;
            }

            return false;
        }
    }
}
