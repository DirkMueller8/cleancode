namespace Logger.Core;

/// <summary>
/// The declaration of an application's log structure: a set of <see cref="LogType"/>s,
/// each retrievable by name (REQ-0002). Immutable and validated on construction:
/// duplicate log-type names are rejected, so a <see cref="Schema"/> instance is always well-formed.
/// </summary>
public sealed class Schema
{
    private readonly Dictionary<string, LogType> logTypes;

    public IReadOnlyCollection<LogType> LogTypes => this.logTypes.Values;

    public Schema(IEnumerable<LogType> logTypes)
    {
        ArgumentNullException.ThrowIfNull(logTypes);

        this.logTypes = new Dictionary<string, LogType>(StringComparer.Ordinal);
        foreach (LogType logType in logTypes)
        {
            if (!this.logTypes.TryAdd(logType.Name, logType))
            {
                throw new ArgumentException(
                    $"Duplicate log type name '{logType.Name}'.",
                    nameof(logTypes));
            }
        }
    }

    /// <summary>Looks up a log type by exact (case-sensitive) name.</summary>
    public bool TryGetLogType(string name, out LogType? logType) =>
        this.logTypes.TryGetValue(name, out logType);
}
