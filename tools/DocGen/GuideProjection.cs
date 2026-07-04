namespace DocGen;

/// <summary>
/// Decides which requirements and which sections reach the user guide. Pure and concrete (no seam
/// needed). A requirement is included when it is <c>user_facing</c> and not <c>out-of-scope-infra</c>;
/// the included sections are the user-facing headings, in a fixed order. The U/I split is by heading
/// name (fixed by the requirement template), so requirement files need no inline tags.
/// </summary>
public sealed class GuideProjection
{
    private const string ExcludedScope = "out-of-scope-infra";

    private static readonly string[] UserSectionOrder = { "Summary", "Requirement", "Worked example" };

    public bool Includes(RequirementDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        return document.UserFacing
            && !string.Equals(document.Scope, ExcludedScope, StringComparison.Ordinal);
    }

    public IReadOnlyList<Section> UserFacingSections(RequirementDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var selected = new List<Section>();
        foreach (string heading in UserSectionOrder)
        {
            Section? section = document.Sections
                .FirstOrDefault(s => string.Equals(s.Heading, heading, StringComparison.Ordinal));
            if (section is not null)
            {
                selected.Add(section);
            }
        }

        return selected;
    }
}
