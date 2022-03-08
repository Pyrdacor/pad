using pad.core.interfaces;
using pad.core.util;

namespace pad.core.extensions
{
    internal static class DataReaderExtensions
    {
        public static string ReadDisplacement(this IDataReader dataReader)
        {
            return WordConverter.AsSigned(dataReader.ReadWord()).ToSignedHexString();
        }
    }
}
