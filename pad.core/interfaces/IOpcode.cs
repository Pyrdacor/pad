namespace pad.core.interfaces
{
    public interface IOpcode
    {
        bool TryMatch(IDataReader reader, out string asm, out List<uint> absoluteLongAddresses);
    }

    public interface IBranchOpcode : IOpcode
    {
        int Displacement { get; }
    }

    public interface IJumpOpcode : IOpcode
    {

    }

    public interface ISimple16BitOpcode
    {
        string ConvertToAsm(IDataReader reader);
    }
}
