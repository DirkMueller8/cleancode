using Logger.Core;

namespace Logger.Services;

/// <summary>
/// An append-only store of recorded events (REQ-0025). Events can be appended and read, but never
/// modified or removed — append-only is enforced by the <b>shape</b> of this interface (there is no
/// update or delete method). Real persistence is out of scope; see <see cref="InMemoryLogStore"/>.
/// </summary>
public interface ILogStore
{
    void Append(LogEvent logEvent);

    IReadOnlyList<LogEvent> All();
}
