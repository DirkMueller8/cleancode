using System.Text;

namespace DocGen;

/// <summary>
/// Parses a raw requirement into its frontmatter fields and body sections. Pure and concrete — there
/// is nothing to substitute, so it needs no interface. Returns <c>null</c> for anything that isn't a
/// requirement (no YAML frontmatter with an <c>id</c>), so non-requirement files are simply skipped.
/// Frontmatter is read as simple <c>key: value</c> lines — no external YAML dependency.
/// </summary>
public sealed class RequirementParser
{
    public RequirementDocument? Parse(RawRequirement raw)
    {
        ArgumentNullException.ThrowIfNull(raw);

        string[] lines = raw.Text.Replace("\r\n", "\n").Split('\n');
        if (lines.Length == 0 || lines[0].Trim() != "---")
        {
            return null;
        }

        var frontmatter = new Dictionary<string, string>(StringComparer.Ordinal);
        int index = 1;
        for (; index < lines.Length; index++)
        {
            if (lines[index].Trim() == "---")
            {
                index++;
                break;
            }

            int colon = lines[index].IndexOf(':');
            if (colon <= 0)
            {
                continue;
            }

            string key = lines[index][..colon].Trim();
            string value = lines[index][(colon + 1)..].Trim().Trim('"');
            frontmatter[key] = value;
        }

        if (!frontmatter.TryGetValue("id", out string? id))
        {
            return null;
        }

        bool userFacing = bool.TryParse(frontmatter.GetValueOrDefault("user_facing"), out bool parsed) && parsed;
        return new RequirementDocument(
            id,
            frontmatter.GetValueOrDefault("title", id),
            frontmatter.GetValueOrDefault("doc_chapter", "Uncategorized"),
            userFacing,
            frontmatter.GetValueOrDefault("scope", string.Empty),
            ParseSections(lines, index));
    }

    private static IReadOnlyList<Section> ParseSections(string[] lines, int start)
    {
        var sections = new List<Section>();
        string? heading = null;
        var body = new StringBuilder();

        for (int i = start; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                AddSection(sections, heading, body);
                heading = CleanHeading(line[3..]);
                body.Clear();
            }
            else if (heading is not null)
            {
                body.Append(line).Append('\n');
            }
        }

        AddSection(sections, heading, body);
        return sections;
    }

    private static void AddSection(List<Section> sections, string? heading, StringBuilder body)
    {
        if (heading is not null)
        {
            sections.Add(new Section(heading, body.ToString().Trim()));
        }
    }

    private static string CleanHeading(string raw)
    {
        int comment = raw.IndexOf("<!--", StringComparison.Ordinal);
        if (comment >= 0)
        {
            raw = raw[..comment];
        }

        return raw.Trim();
    }
}
