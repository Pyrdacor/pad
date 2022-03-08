using pad.core.serialization;

namespace pad.core.interfaces
{
	public interface IHunk
	{
		HunkType Type { get; }
		uint Size { get; }
		uint MemoryFlags { get; }
	}
}
