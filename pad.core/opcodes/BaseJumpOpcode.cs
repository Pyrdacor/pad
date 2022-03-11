using pad.core.interfaces;

namespace pad.core.opcodes
{
    internal abstract class BaseJumpOpcode : BaseOpcode, IJumpOpcode
    {
        static string GetJumpTarget(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var arg = ParseArg(header, 10, dataReader, 1, addresses,
                AddressingModes.Address | AddressingModes.AddressWithDisplacement |
                AddressingModes.AddressWithIndex | AddressingModes.AbsoluteShort |
                AddressingModes.AbsoluteLong | AddressingModes.PCWithDisplacement |
                AddressingModes.PCWithIndex, false, true);
            return arg;
        }

        protected BaseJumpOpcode(Func<ushort, bool> matcher, Func<ushort, IDataReader, KeyValuePair<string, Dictionary<string, Reference>>> asmProvider,
            Func<ushort, int> binarySizeProvider)
            : base(matcher, asmProvider, binarySizeProvider)
        {

        }

        protected BaseJumpOpcode(ushort mask, ushort value, Func<ushort, IDataReader, KeyValuePair<string, Dictionary<string, Reference>>> asmProvider,
            Func<ushort, int> binarySizeProvider)
            : base(mask, value, asmProvider, binarySizeProvider)
        {

        }

        public string JumpTarget { get; private set; } = "";
        public abstract bool SubRoutine { get; }

        public override bool TryMatch(IDataReader reader, out string asm, out Dictionary<string, Reference> references, out int binarySize)
        {
            int position = reader.Position;
            var header = reader.ReadWord();
            JumpTarget = GetJumpTarget(header, reader);
            reader.Position = position;

            return base.TryMatch(reader, out asm, out references, out binarySize);
        }
    }
}
