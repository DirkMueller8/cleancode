namespace DocGen;

/// <summary>
/// Orchestrates the pipeline: read requirements → parse → project (filter + select user sections) →
/// render. Returns the guide text (writing it to disk is the console program's job — this stays pure
/// and testable). Collaborators are injected (DIP); the composition root wires them.
/// </summary>
public sealed class UserGuideGenerator
{
    private readonly IRequirementSource source;
    private readonly RequirementParser parser;
    private readonly GuideProjection projection;
    private readonly IGuideRenderer renderer;

    public UserGuideGenerator(
        IRequirementSource source,
        RequirementParser parser,
        GuideProjection projection,
        IGuideRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(parser);
        ArgumentNullException.ThrowIfNull(projection);
        ArgumentNullException.ThrowIfNull(renderer);
        this.source = source;
        this.parser = parser;
        this.projection = projection;
        this.renderer = renderer;
    }

    public string Generate()
    {
        var entries = new List<GuideEntry>();
        foreach (RawRequirement raw in this.source.Read())
        {
            RequirementDocument? document = this.parser.Parse(raw);
            if (document is null || !this.projection.Includes(document))
            {
                continue;
            }

            entries.Add(new GuideEntry(
                document.Id,
                document.Chapter,
                document.Title,
                this.projection.UserFacingSections(document)));
        }

        return this.renderer.Render(entries);
    }
}
