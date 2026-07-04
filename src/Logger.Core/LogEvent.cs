namespace Logger.Core;

/// <summary>
/// One event submitted to the Logger for a given LogType (REQ-0005): a set of field name → value
/// pairs. Values are held in their canonical <b>text</b> form (DSS §8: log data maps to/from a
/// canonical text representation); type checking against the declared <see cref="FieldType"/> happens
/// in <see cref="SchemaValidator"/>. Field names are compared case-sensitively (glossary → <i>name</i>).
/// </summary>
public sealed class LogEvent
{
    private readonly Dictionary<string, string> values;

    public string LogTypeName { get; }

    public IReadOnlyDictionary<string, string> Values => this.values;

    public LogEvent(string logTypeName, IReadOnlyDictionary<string, string> values)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(logTypeName);
        ArgumentNullException.ThrowIfNull(values);
        this.LogTypeName = logTypeName;
        this.values = new Dictionary<string, string>(values, StringComparer.Ordinal);
    }
}
