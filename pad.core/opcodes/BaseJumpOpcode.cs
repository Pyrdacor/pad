using pad.core.interfaces;

namespace pad.core.opcodes
{
    internal class BaseJumpOpcode : BaseOpcode, IJumpOpcode
    {
        static string GetJumpTarget(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, uint>();
            var arg = ParseArg(header, 10, dataReader, 1, addresses,
                AddressingModes.Address | AddressingModes.AddressWithDisplacement |
                AddressingModes.AddressWithIndex | AddressingModes.AbsoluteShort |
                AddressingModes.AbsoluteLong | AddressingModes.PCWithDisplacement |
                AddressingModes.PCWithIndex, "", false, true);
            return arg;
        }

        protected BaseJumpOpcode(Func<ushort, bool> matcher, Func<ushort, IDataReader, KeyValuePair<string, Dictionary<string, uint>>> asmProvider)
            : base(matcher, asmProvider)
        {

        }

        protected BaseJumpOpcode(ushort mask, ushort value, Func<ushort, IDataReader, KeyValuePair<string, Dictionary<string, uint>>> asmProvider)
            : base(mask, value, asmProvider)
        {

        }

        public string JumpTarget { get; private set; } = "";

        public override bool TryMatch(IDataReader reader, out string asm, out Dictionary<string, uint> references)
        {
            int position = reader.Position;
            var header = reader.ReadWord();
            JumpTarget = GetJumpTarget(header, reader);
            reader.Position = position;

            return base.TryMatch(reader, out asm, out references);
        }
    }
}
