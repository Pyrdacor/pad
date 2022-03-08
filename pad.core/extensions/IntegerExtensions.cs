namespace pad.core.extensions
{
    internal static class IntegerExtensions
    {
        public static string ToSignedHexString(this sbyte value)
        {
            return value >= 0 ? $"${value:x2}" : $"-${-value:x2}";
        }

        public static string ToSignedHexString(this short value)
        {
            return value >= 0 ? $"${value:x4}" : $"-${-value:x4}";
        }
    }
}
