namespace pad.core.opcodes
{
    internal enum Condition
    {
        T, // true
        F, // false
        HI, // higher than
        LS, // lower than or same
        CC, // carry clear
        CS, // carry set
        NE, // not equal
        EQ, // equal
        VC, // overflow clear
        VS, // overflow set
        PL, // plus
        MI, // minus
        GE, // greater than or equal
        LT, // less than
        GT, // greater than        
        LE  // less than or equal        
    }
}
