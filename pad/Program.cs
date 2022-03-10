using pad.core;
using pad.core.serialization;

var reader = new DataReader(File.ReadAllBytes(@"D:\Programmierung\C#\Projects\Ambermoon\Ambermoon-Advanced\english\Amberfiles\AM2_CPU_DEP"));
var hunks = AmigaExecutable.Read(reader);

var codeData = new DataReader(((Hunk)hunks[0]).Data!);
var relocs = ((Reloc32Hunk)hunks[1]).Entries;

var codeHunk = new CodeHunk(0, codeData, relocs);

uint offset = 0;
List<uint> codeOffsets = new();

foreach (var hunk in hunks)
{
    if (hunk.Type != HunkType.Reloc32 && hunk.Type != HunkType.End)
    {
        codeOffsets.Add(offset);
        offset += hunk.Size * 4;
    }
}

codeHunk.Process(codeOffsets);

File.WriteAllText(@"D:\Programmierung\C#\Projects\pad\test.asm", codeHunk.Asm);