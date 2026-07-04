namespace DocGen;

/// <summary>An included requirement, reduced to what the guide renders: id (for ordering), chapter,
/// title, and only the user-facing sections.</summary>
public sealed record GuideEntry(string Id, string Chapter, string Title, IReadOnlyList<Section> Sections);
