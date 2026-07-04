namespace Logger.Core;

/// <summary>One condition of a filtered-log query: a field and the value to match (REQ-0019).</summary>
public sealed record QueryCondition(string FieldName, string Value);
