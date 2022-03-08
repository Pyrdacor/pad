namespace pad.core.interfaces
{
    public interface IOpcode
    {
        bool TryMatch(IDataReader reader, out string asm, out List<uint> absoluteLongAddresses);
    }

    public interface ISimple16BitOpcode
    {
        string ConvertToAsm(IDataReader reader);
    }
}
