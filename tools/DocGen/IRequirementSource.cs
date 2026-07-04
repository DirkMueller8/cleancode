namespace DocGen;

/// <summary>
/// Supplies raw requirement files. An injected seam so tests can feed in-memory requirements instead
/// of reading the filesystem.
/// </summary>
public interface IRequirementSource
{
    IEnumerable<RawRequirement> Read();
}
