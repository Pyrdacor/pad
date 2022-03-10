namespace pad.core.interfaces
{
    public interface IOpcode
    {
        bool TryMatch(IDataReader reader, out string asm, out Dictionary<string, uint> references, out int binarySize);
    }

    public interface IBranchOpcode : IOpcode
    {
        int Displacement { get; }
        bool Unconditional { get; }
    }

    public interface IJumpOpcode : IOpcode
    {
        string JumpTarget { get; }
    }

    public interface ISimple16BitOpcode
    {
        string ConvertToAsm(IDataReader reader);
        int Size { get; }
    }
}
