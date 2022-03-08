using System.Runtime.InteropServices;

namespace pad.core.util
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct ByteConverter
    {
        [FieldOffset(0)] sbyte SbyteValue;
        [FieldOffset(0)] byte ByteValue;

        public static sbyte AsSigned(byte source)
        {
            var converter = new ByteConverter();
            converter.ByteValue = source;
            return converter.SbyteValue;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct WordConverter
    {
        [FieldOffset(0)] short ShortValue;
        [FieldOffset(0)] ushort WordValue;

        public static short AsSigned(ushort source)
        {
            var converter = new WordConverter();
            converter.WordValue = source;
            return converter.ShortValue;
        }
    }
}
