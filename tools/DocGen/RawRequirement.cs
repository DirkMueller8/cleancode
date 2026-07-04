namespace DocGen;

/// <summary>A requirement file as read from a source: its name and full text.</summary>
public sealed record RawRequirement(string Name, string Text);
