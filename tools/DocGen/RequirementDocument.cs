namespace DocGen;

/// <summary>A parsed requirement: the frontmatter fields the guide needs, plus its body sections.</summary>
public sealed class RequirementDocument
{
    public string Id { get; }

    public string Title { get; }

    public string Chapter { get; }

    public bool UserFacing { get; }

    public string Scope { get; }

    public IReadOnlyList<Section> Sections { get; }

    public RequirementDocument(
        string id,
        string title,
        string chapter,
        bool userFacing,
        string scope,
        IReadOnlyList<Section> sections)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(sections);
        this.Id = id;
        this.Title = title;
        this.Chapter = chapter;
        this.UserFacing = userFacing;
        this.Scope = scope;
        this.Sections = sections;
    }
}
