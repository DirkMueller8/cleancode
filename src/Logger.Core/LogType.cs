namespace Logger.Core;

/// <summary>
/// A named category of log event (e.g. "login") and the fields it may carry (REQ-0002).
/// Immutable and validated on construction: duplicate field names are rejected, so a
/// <see cref="LogType"/> instance is always well-formed.
/// </summary>
public sealed class LogType
{
    private readonly Dictionary<string, FieldDefinition> fields;

    public string Name { get; }

    public IReadOnlyCollection<FieldDefinition> Fields => this.fields.Values;

    public LogType(string name, IEnumerable<FieldDefinition> fields)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(fields);

        this.Name = name;
        this.fields = new Dictionary<string, FieldDefinition>(StringComparer.Ordinal);
        foreach (FieldDefinition field in fields)
        {
            if (!this.fields.TryAdd(field.Name, field))
            {
                throw new ArgumentException(
                    $"Duplicate field name '{field.Name}' in log type '{name}'.",
                    nameof(fields));
            }
        }
    }

    /// <summary>Looks up a field by exact (case-sensitive) name.</summary>
    public bool TryGetField(string name, out FieldDefinition? field) =>
        this.fields.TryGetValue(name, out field);
}
