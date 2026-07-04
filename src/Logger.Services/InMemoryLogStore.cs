using Logger.Core;

namespace Logger.Services;

/// <summary>
/// In-memory <see cref="ILogStore"/> (REQ-0025): holds events in append order. <see cref="All"/> returns
/// a snapshot so callers can't mutate the store's contents. The in-memory stub for the "core + service
/// stubs" scope; real persistence (files, encryption at rest, RAID) is out of scope.
/// </summary>
public sealed class InMemoryLogStore : ILogStore
{
    private readonly List<LogEvent> events = new();

    public void Append(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        this.events.Add(logEvent);
    }

    public IReadOnlyList<LogEvent> All() => this.events.ToArray();
}
