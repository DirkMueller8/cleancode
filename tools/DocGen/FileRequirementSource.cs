namespace DocGen;

/// <summary>
/// Reads requirement files from a directory. Only files named <c>NNNN-*.md</c> are requirements;
/// TEMPLATE, README, glossary and other docs are ignored by that naming rule.
/// </summary>
public sealed class FileRequirementSource : IRequirementSource
{
    private readonly string directory;

    public FileRequirementSource(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        this.directory = directory;
    }

    public IEnumerable<RawRequirement> Read()
    {
        foreach (string path in Directory.EnumerateFiles(this.directory, "*.md"))
        {
            string name = Path.GetFileName(path);
            if (IsRequirementFile(name))
            {
                yield return new RawRequirement(name, File.ReadAllText(path));
            }
        }
    }

    private static bool IsRequirementFile(string name) =>
        name.Length >= 5
        && char.IsAsciiDigit(name[0]) && char.IsAsciiDigit(name[1])
        && char.IsAsciiDigit(name[2]) && char.IsAsciiDigit(name[3])
        && name[4] == '-';
}
