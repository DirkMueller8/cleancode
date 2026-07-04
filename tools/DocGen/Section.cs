namespace DocGen;

/// <summary>One body section of a requirement: its heading (e.g. "Summary") and its content.</summary>
public sealed record Section(string Heading, string Body);
