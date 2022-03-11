using pad.core.interfaces;
using pad.core.util;

namespace pad.core.opcodes
{
    // TODO: Replace offsets with label as well!
    internal abstract class BaseBranchOpcode : BaseOpcode, IBranchOpcode
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

        protected BaseBranchOpcode(Func<ushort, bool> matcher, Func<ushort, IDataReader, string> asmProvider, bool unconditional,
            Func<ushort, int> binarySizeProvider)
            : base(matcher, asmProvider, binarySizeProvider)
        {
            Unconditional = unconditional;
        }

        protected BaseBranchOpcode(ushort mask, ushort value, Func<ushort, IDataReader, string> asmProvider, bool unconditional,
            Func<ushort, int> binarySizeProvider)
            : base(mask, value, asmProvider, binarySizeProvider)
        {
            Unconditional = unconditional;
        }

        public int Displacement { get; private set; }
        public bool Unconditional { get; }

        public override bool TryMatch(IDataReader reader, out string asm, out Dictionary<string, Reference> references, out int binarySize)
        {
            if (base.TryMatch(reader, out asm, out references, out binarySize))
            {
                reader.Position -= binarySize;
                Displacement = GetDisplacement(binarySize < 4 ? (uint)reader.ReadWord() << 16 : reader.ReadDword());
                return true;
            }

            return false;
        }
    }
}
