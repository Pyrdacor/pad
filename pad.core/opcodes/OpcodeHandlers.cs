namespace pad.core.opcodes
{
    public delegate void AsmOutputHandler(string asm);
    public delegate void BranchHandler(uint targetLocation, bool unconditional);
    public delegate void JumpHandler(uint jumpCallOffset, string jumpTarget);
    public delegate void ReferenceHandler(Dictionary<string, uint> references);

    public record OpcodeHandlers
    (
        AsmOutputHandler AsmOutputHandler, BranchHandler BranchHandler,
        JumpHandler JumpHandler, ReferenceHandler ReferenceHandler
    );
}
