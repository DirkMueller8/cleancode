using DocGen;

namespace DocGen.Tests;

/// <summary>
/// Tests for REQ-0050 — generating the user guide from requirement files.
/// </summary>
public sealed class UserGuideGeneratorTests
{
    // AC: only user_facing requirements appear.
    [Fact]
    public void Generate_IncludesOnlyUserFacingRequirements()
    {
        string guide = Generator(
            Req("REQ-0002", "Alpha", "Defining", userFacing: true),
            Req("REQ-0003", "Beta", "Defining", userFacing: false)).Generate();

        Assert.Contains("Alpha", guide);
        Assert.DoesNotContain("Beta", guide);
    }

    // AC: out-of-scope-infra requirements are excluded.
    [Fact]
    public void Generate_ExcludesOutOfScopeInfra()
    {
        string guide = Generator(
            Req("REQ-0002", "Alpha", "Defining", userFacing: true),
            Req("REQ-0003", "Gamma", "Defining", userFacing: true, scope: "out-of-scope-infra")).Generate();

        Assert.Contains("Alpha", guide);
        Assert.DoesNotContain("Gamma", guide);
    }

    // AC: only U sections appear; internal sections do not.
    [Fact]
    public void Generate_EmitsUserSections_ButNotInternalSections()
    {
        string guide = Generator(Req("REQ-0002", "Alpha", "Defining", userFacing: true)).Generate();

        Assert.Contains("SUMMARY_REQ-0002", guide);
        Assert.Contains("REQUIREMENT_REQ-0002", guide);
        Assert.Contains("WORKED_REQ-0002", guide);
        Assert.DoesNotContain("INTERNAL_CRITERION", guide);
        Assert.DoesNotContain("INTERNAL_DESIGN", guide);
    }

    // AC: grouped by chapter (chapters ordered by lowest id), entries ordered by id.
    [Fact]
    public void Generate_GroupsByChapter_OrderedById()
    {
        string guide = Generator(
            Req("REQ-0007", "Gee", "Filtering", userFacing: true),
            Req("REQ-0003", "Bee", "Defining", userFacing: true),
            Req("REQ-0002", "Ay", "Defining", userFacing: true)).Generate();

        Assert.True(guide.IndexOf("## Defining", StringComparison.Ordinal)
                  < guide.IndexOf("## Filtering", StringComparison.Ordinal));
        Assert.True(guide.IndexOf("### Ay", StringComparison.Ordinal)
                  < guide.IndexOf("### Bee", StringComparison.Ordinal));
    }

    // AC: deterministic.
    [Fact]
    public void Generate_IsDeterministic()
    {
        UserGuideGenerator generator = Generator(
            Req("REQ-0002", "Alpha", "Defining", userFacing: true),
            Req("REQ-0007", "Gee", "Filtering", userFacing: true));

        Assert.Equal(generator.Generate(), generator.Generate());
    }

    [Fact]
    public void Parser_ParsesFrontmatterAndSections()
    {
        RequirementDocument? doc = new RequirementParser().Parse(Req("REQ-0002", "Alpha", "Defining", userFacing: true));

        Assert.NotNull(doc);
        Assert.Equal("REQ-0002", doc!.Id);
        Assert.Equal("Alpha", doc.Title);
        Assert.Equal("Defining", doc.Chapter);
        Assert.True(doc.UserFacing);
        Assert.Contains(doc.Sections, s => s.Heading == "Summary");
    }

    // AC: non-requirement files are ignored (no frontmatter -> null).
    [Fact]
    public void Parser_NonRequirementFile_ReturnsNull()
    {
        RequirementDocument? doc = new RequirementParser()
            .Parse(new RawRequirement("README.md", "# Readme\n\nNot a requirement."));

        Assert.Null(doc);
    }

    [Fact]
    public void Renderer_NoEntries_ProducesPlaceholder()
    {
        string guide = new MarkdownGuideRenderer().Render(Array.Empty<GuideEntry>());

        Assert.Contains("No user-facing requirements", guide);
    }

    private static UserGuideGenerator Generator(params RawRequirement[] requirements) =>
        new(new StubSource(requirements), new RequirementParser(), new GuideProjection(), new MarkdownGuideRenderer());

    private static RawRequirement Req(
        string id,
        string title,
        string chapter,
        bool userFacing,
        string scope = "now")
    {
        string text = string.Join('\n', new[]
        {
            "---",
            $"id: {id}",
            $"title: {title}",
            $"doc_chapter: \"{chapter}\"",
            $"scope: {scope}",
            $"user_facing: {(userFacing ? "true" : "false")}",
            "---",
            string.Empty,
            "## Summary",
            $"SUMMARY_{id}",
            string.Empty,
            "## Requirement",
            $"REQUIREMENT_{id}",
            string.Empty,
            "## Worked example",
            "```",
            $"WORKED_{id}",
            "```",
            string.Empty,
            "## Acceptance criteria",
            $"INTERNAL_CRITERION_{id}",
            string.Empty,
            "## Design notes",
            $"INTERNAL_DESIGN_{id}",
        });

        return new RawRequirement($"{id}.md", text);
    }

    private sealed class StubSource : IRequirementSource
    {
        private readonly IReadOnlyList<RawRequirement> requirements;

        public StubSource(IReadOnlyList<RawRequirement> requirements) => this.requirements = requirements;

        public IEnumerable<RawRequirement> Read() => this.requirements;
    }
}
