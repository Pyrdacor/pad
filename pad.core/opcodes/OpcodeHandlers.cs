namespace pad.core.opcodes
{
    public delegate void AsmOutputHandler(string asm);
    public delegate void BranchHandler(uint targetLocation, bool unconditional);
    public delegate void JumpHandler(uint jumpCallOffset, string jumpTarget, bool subRoutine);
    public delegate void ReferenceHandler(Dictionary<string, Reference> references);
    public delegate void OpcodeSizeHandler(int size);

    public record OpcodeHandlers
    (
        AsmOutputHandler AsmOutputHandler, BranchHandler BranchHandler,
        JumpHandler JumpHandler, ReferenceHandler ReferenceHandler,
        OpcodeSizeHandler OpcodeSizeHandler
    );
}
