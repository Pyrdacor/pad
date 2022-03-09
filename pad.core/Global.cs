global using word = System.UInt16;
global using dword = System.UInt32;
global using qword = System.UInt64;

public static class Global
{
    public static string AddressRegisterName(int index)
    {
        return index switch
        {
            >= 0 and < 7 => $"A{index}",
            7 => "SP",
            _ => throw new IndexOutOfRangeException($"Invalid address register index {index}.")
        };
    }
}