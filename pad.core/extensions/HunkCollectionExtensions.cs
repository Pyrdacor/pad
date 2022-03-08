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
    }
}
