using System.Text;

namespace DocGen;

/// <summary>
/// Renders guide entries as Markdown: chapters (ordered by their lowest id) as <c>##</c>, requirements
/// (ordered by id) as <c>###</c>, then their user-facing section bodies. Deterministic.
/// </summary>
public sealed class MarkdownGuideRenderer : IGuideRenderer
{
    public string Render(IReadOnlyList<GuideEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        var output = new StringBuilder();
        output.Append("# Logger — User Guide\n\n");
        output.Append("_Generated from the requirements by DocGen. Do not edit by hand._\n");

        if (entries.Count == 0)
        {
            output.Append("\n_No user-facing requirements yet._\n");
            return output.ToString();
        }

        // Sorting by id first, then grouping, yields chapters in lowest-id order with id-ordered entries.
        IEnumerable<GuideEntry> ordered = entries.OrderBy(e => e.Id, StringComparer.Ordinal);
        foreach (IGrouping<string, GuideEntry> chapter in ordered.GroupBy(e => e.Chapter, StringComparer.Ordinal))
        {
            output.Append("\n## ").Append(chapter.Key).Append('\n');
            foreach (GuideEntry entry in chapter)
            {
                output.Append("\n### ").Append(entry.Title).Append('\n');
                foreach (Section section in entry.Sections)
                {
                    output.Append('\n').Append(section.Body).Append('\n');
                }
            }
        }

        return output.ToString();
    }
}
