namespace DocGen;

/// <summary>
/// Renders the projected guide entries into the final document. An injected seam so the output format
/// (Markdown today) can change without touching the generation pipeline.
/// </summary>
public interface IGuideRenderer
{
    string Render(IReadOnlyList<GuideEntry> entries);
}
