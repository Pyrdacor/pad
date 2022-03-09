using pad.core.opcodes;

namespace pad.core.extensions
{
    internal static class AddressingModeExtensions
    {
        public static AddressingModes Exclude(this AddressingModes addressingModes, AddressingModes exclude)
        {
            return addressingModes & ~exclude;
        }

        public static AddressingModes Exclude(this AddressingModes addressingModes, AddressingModes excludeFirst, AddressingModes excludeSecond,
            params AddressingModes[] excludeMore)
        {
            addressingModes &= ~excludeFirst;
            addressingModes &= ~excludeSecond;

            foreach (var more in excludeMore)
                addressingModes &= ~more;

            return addressingModes;
        }
    }
}
