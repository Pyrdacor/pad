using pad.core.extensions;
using pad.core.interfaces;

namespace pad.core.opcodes
{
    /// <summary>
    /// MOVEP - Move from data register to periphery or vice versa
    /// 
    /// This instruction was originally designed for interfacing 8-bit
    /// peripherals on a 16-bit data bus, such as the MC68000 bus.
    /// 
    /// Memory at An + d:
    /// +-+-+
    /// | |X| <-> Dn[24..31]
    /// +-+-+
    /// | |X| <-> Dn[16..23]
    /// +-+-+
    /// | |X| <-> Dn[8..15]
    /// +-+-+
    /// | |X| <-> Dn[0..7]
    /// +-+-+
    /// 
    /// MOVEP Dn,(d,An)
    /// MOVEP (d,An),Dn
    /// </summary>
    internal class OpcodeMovep : BaseOpcode
    {
        public OpcodeMovep()
            : base(0xf138, 0x0108, ToAsm, _ => 4)
        {

        }

        static string ToAsm(ushort header, IDataReader dataReader)
        {
            var dataRegister = $"D{(header >> 9) & 0x7}";
            var addrRegister = Global.AddressRegisterName(header & 0x7);
            var displacement = dataReader.ReadDisplacement();

            if ((header & 0x80) == 0) // (d,An) -> Dn
                return $"MOVEP ({displacement},{addrRegister}),{dataRegister}";
            else // Dn -> (d,An)
                return $"MOVEP {dataRegister},({displacement},{addrRegister})";
        }
    }
}
