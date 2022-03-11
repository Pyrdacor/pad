using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// MOVE from SR - Copy data from status register to destination
    /// 
    /// MOVE SR,&lt;ea&gt;
    /// Size: Word
    /// </summary>
    internal class OpcodeMoveFromSr : BaseOpcode
    {
        public OpcodeMoveFromSr()
            : base(0xffc0, 0x40c0, ToAsm, header => SizeWithArg(header, 0))
        {

        }

        static KeyValuePair<string, Dictionary<string, Reference>> ToAsm(ushort header, IDataReader dataReader)
        {
            var addresses = new Dictionary<string, Reference>();
            var arg = ParseArg(header, 10, dataReader, 2, addresses, AddressingModes.Default);

            return KeyValuePair.Create($"MOVE.W SR,{arg}", addresses);
        }
    }
}
