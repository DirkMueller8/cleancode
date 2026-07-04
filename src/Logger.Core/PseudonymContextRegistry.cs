using System.Security.Cryptography;

namespace Logger.Core;

/// <summary>
/// Holds one <see cref="PseudonymContext"/> per (user, log) key and manages its lifecycle
/// (REQ-0016/0017/0018). A context can be cleared on request (REQ-0017), and any context idle for at
/// least the idle timeout (24h by default) is discarded on the next access (REQ-0018). Time comes from
/// an injected <see cref="IClock"/> so expiry is testable. Each context is created with a fresh random
/// salt (REQ-0010).
/// </summary>
public sealed class PseudonymContextRegistry
{
    private static readonly TimeSpan DefaultIdleTimeout = TimeSpan.FromHours(24);

    private readonly IHasher hasher;
    private readonly IClock clock;
    private readonly TimeSpan idleTimeout;
    private readonly Dictionary<(string User, string Log), Entry> entries = new();

    public PseudonymContextRegistry(IHasher hasher, IClock clock)
        : this(hasher, clock, DefaultIdleTimeout)
    {
    }

    public PseudonymContextRegistry(IHasher hasher, IClock clock, TimeSpan idleTimeout)
    {
        ArgumentNullException.ThrowIfNull(hasher);
        ArgumentNullException.ThrowIfNull(clock);
        if (idleTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(idleTimeout), "The idle timeout must be positive.");
        }

        this.hasher = hasher;
        this.clock = clock;
        this.idleTimeout = idleTimeout;
    }

    /// <summary>Number of live contexts (after any due expiry) — for inspection/tests.</summary>
    public int Count
    {
        get
        {
            this.PurgeExpired();
            return this.entries.Count;
        }
    }

    /// <summary>Gets the context for the (user, log), creating a fresh one if absent or expired.</summary>
    public IPseudonymContext GetContext(string user, string log)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(log);

        this.PurgeExpired();

        (string, string) key = (user, log);
        if (!this.entries.TryGetValue(key, out Entry? entry))
        {
            entry = new Entry(this.CreateContext());
            this.entries.Add(key, entry);
        }

        entry.LastAccess = this.clock.UtcNow;
        return entry.Context;
    }

    /// <summary>Discards the (user, log) context so the next access starts fresh (REQ-0017).</summary>
    public void Clear(string user, string log)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(log);
        this.entries.Remove((user, log));
    }

    private void PurgeExpired()
    {
        DateTimeOffset now = this.clock.UtcNow;
        List<(string User, string Log)> expired = this.entries
            .Where(pair => now - pair.Value.LastAccess >= this.idleTimeout)
            .Select(pair => pair.Key)
            .ToList();

        foreach ((string, string) key in expired)
        {
            this.entries.Remove(key);
        }
    }

    private PseudonymContext CreateContext() => new(this.hasher, GenerateSalt());

    private static string GenerateSalt() => Convert.ToHexString(RandomNumberGenerator.GetBytes(16));

    private sealed class Entry
    {
        public Entry(PseudonymContext context) => this.Context = context;

        public PseudonymContext Context { get; }

        public DateTimeOffset LastAccess { get; set; }
    }
}
