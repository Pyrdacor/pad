using pad.core.interfaces;

namespace pad.core.serialization
{
	public struct Hunk : IHunk
	{
		public Hunk(HunkType hunkType, uint memoryFlags = 0, byte[]? data = null, uint? numEntries = null)
		{
			Type = hunkType;
			MemoryFlags = memoryFlags;

			if (Type == HunkType.Code || Type == HunkType.Data)
			{
				Data = data ?? throw new InvalidOperationException("Code/data hunks need non-null data value.");

				if (numEntries != null)
				{
					int size = (int)numEntries.Value * 4;

					if (size > Data.Length)
						throw new ArgumentOutOfRangeException("numEntries * 4 exceeds data size.");

					if (size < Data.Length)
						Data = Data.Take(size).ToArray();
				}
				else if (Data.Length % 4 != 0)
					throw new InvalidOperationException("Hunk data size must be a multiple of 4.");

				Size = NumEntries = (uint)Data.Length / 4;
			}
			else if (Type == HunkType.BSS)
			{
				Size = NumEntries = numEntries ?? throw new NullReferenceException("BSS hunks need a non-null value for numEntries.");
				Data = null;
			}
			else if (Type == HunkType.Reloc32)
			{
				throw new NotSupportedException("Creating RELOC32 hunks via constructor is not supported.");
			}
			else if (Type == HunkType.End)
			{
				NumEntries = 0;
				Size = 0;
				Data = null;
			}
			else
			{
				throw new NotSupportedException("Not supported or invalid hunk type.");
			}
		}

		public HunkType Type { get; internal set; }
		public uint Size { get; internal set; }
		public uint MemoryFlags { get; internal set; }
		public uint NumEntries;
		public byte[]? Data;
	}

	public struct Reloc32Hunk : IHunk
	{
		public HunkType Type { get; internal set; }
		public uint Size { get; internal set; }
		public uint MemoryFlags { get; internal set; }
		public Dictionary<uint, List<uint>> Entries;
	}
}
