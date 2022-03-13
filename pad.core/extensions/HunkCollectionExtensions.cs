using System.ComponentModel;
using pad.core.interfaces;
using pad.core.serialization;

namespace pad.core.extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HunkCollectionExtensions
    {
        public static IEnumerable<Hunk> GetCodeHunks(this IEnumerable<IHunk> hunks)
            => hunks.Where(h => h.Type == HunkType.Code).Cast<Hunk>();
        public static IEnumerable<Hunk> GetDataHunks(this IEnumerable<IHunk> hunks)
            => hunks.Where(h => h.Type == HunkType.Data).Cast<Hunk>();
        public static IEnumerable<Reloc32Hunk> GetReloc32Hunks(this IEnumerable<IHunk> hunks)
            => hunks.Where(h => h.Type == HunkType.Reloc32).Cast<Reloc32Hunk>();
        public static IEnumerable<Hunk> GetBSSHunks(this IEnumerable<IHunk> hunks)
            => hunks.Where(h => h.Type == HunkType.BSS).Cast<Hunk>();
        public static IEnumerable<Hunk> GetEndHunks(this IEnumerable<IHunk> hunks)
            => hunks.Where(h => h.Type == HunkType.End).Cast<Hunk>();

        public static string GetSectionHeader(this IList<IHunk> hunks, int index)
        {
            if (index == 0)
                return "entry:";

            var type = hunks[index].Type;
            int indexOfType = 1;
            for (int i = 0; i < index; ++i)
            {
                if (hunks[i].Type == type)
                    ++indexOfType;
            }
            string name = $"{type}{indexOfType}";

            return $"SECTION \"{name}\",CODE";
        }
    }
}
