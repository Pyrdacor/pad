namespace pad.core.opcodes
{
    [Flags]
    internal enum AddressingModes
    {
        DataRegister = 0x0001,
        AddressRegister = 0x0002,
        Address = 0x0004,
        AddressWithPost = 0x0008,
        AddressWithPre = 0x0010,
        AddressWithDisplacement = 0x0020,
        AddressWithIndex = 0x0040,
        AbsoluteShort = 0x0080,
        AbsoluteLong = 0x0100,
        PCWithDisplacement = 0x0200,
        PCWithIndex = 0x0400,
        Immediate = 0x0800,
        Default = 0x01fd,
        All = 0xffff
    }
}
