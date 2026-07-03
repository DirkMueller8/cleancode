namespace Logger.Core;

/// <summary>
/// The declaration of an application's log structure: a set of <see cref="LogType"/>s,
/// each retrievable by name (REQ-0002). Immutable and validated on construction, so a
/// <see cref="Schema"/> instance is always well-formed:
/// <list type="bullet">
///   <item>duplicate log-type names are rejected (REQ-0002);</item>
///   <item>every field's <see cref="Disposition"/> must be registered in the supplied
///   <see cref="IFilterRegistry"/> — unknown dispositions are rejected at build time (REQ-0004).</item>
/// </list>
/// </summary>
public sealed class Schema
{
    private readonly Dictionary<string, LogType> logTypes;

    public IReadOnlyCollection<LogType> LogTypes => this.logTypes.Values;

    public Schema(IEnumerable<LogType> logTypes, IFilterRegistry filterRegistry)
    {
        ArgumentNullException.ThrowIfNull(logTypes);
        ArgumentNullException.ThrowIfNull(filterRegistry);

        this.logTypes = new Dictionary<string, LogType>(StringComparer.Ordinal);
        foreach (LogType logType in logTypes)
        {
            if (!this.logTypes.TryAdd(logType.Name, logType))
            {
                throw new ArgumentException(
                    $"Duplicate log type name '{logType.Name}'.",
                    nameof(logTypes));
            }

            ValidateDispositions(logType, filterRegistry);
        }
    }

    /// <summary>Looks up a log type by exact (case-sensitive) name.</summary>
    public bool TryGetLogType(string name, out LogType? logType) =>
        this.logTypes.TryGetValue(name, out logType);

    private static void ValidateDispositions(LogType logType, IFilterRegistry filterRegistry)
    {
        foreach (FieldDefinition field in logType.Fields)
        {
            if (!filterRegistry.IsRegistered(field.Disposition.Name))
            {
                throw new ArgumentException(
                    $"Unknown disposition '{field.Disposition.Name}' on field '{field.Name}' " +
                    $"in log type '{logType.Name}'.",
                    nameof(logType));
            }
        }
    }
}
