namespace Logger.Core;

/// <summary>
/// Supplies the current time. Injected (DIP) so time-dependent behaviour — like the 24h context
/// expiry (REQ-0018) — is deterministic and testable, rather than reading the system clock directly
/// (ISO/IEC 24772-1 [CCI]).
/// </summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
