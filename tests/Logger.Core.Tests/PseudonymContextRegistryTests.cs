using Logger.Core;
using static Logger.Core.Tests.Build;

namespace Logger.Core.Tests;

/// <summary>
/// Tests for the pseudonym-context lifecycle: clearing (REQ-0017) and 24h idle expiry (REQ-0018).
/// </summary>
public sealed class PseudonymContextRegistryTests
{
    private static readonly DateTimeOffset Start = new(2026, 1, 1, 9, 0, 0, TimeSpan.Zero);

    private static PseudonymContextRegistry Registry(FakeClock clock) => new(Sha256, clock);

    // Same key returns the same live context, so mappings persist across accesses.
    [Fact]
    public void GetContext_SameKey_SharesMappings()
    {
        var registry = Registry(new FakeClock(Start));

        Assert.Equal(1, registry.GetContext("u", "login").SequenceFor("USER", "SAM"));
        Assert.Equal(2, registry.GetContext("u", "login").SequenceFor("USER", "BOB"));
        Assert.Equal(1, registry.GetContext("u", "login").SequenceFor("USER", "SAM")); // still 1
    }

    // Different keys are independent (ties REQ-0016).
    [Fact]
    public void GetContext_DifferentKeys_AreIndependent()
    {
        var registry = Registry(new FakeClock(Start));

        registry.GetContext("u1", "login").SequenceFor("USER", "SAM"); // u1: SAM -> 1
        int u2Sam = registry.GetContext("u2", "login").SequenceFor("USER", "SAM"); // u2: SAM -> 1

        Assert.Equal(1, u2Sam);
    }

    // REQ-0017: clearing discards the context so numbering restarts.
    [Fact]
    public void Clear_DiscardsContext_SoMappingsReset()
    {
        var registry = Registry(new FakeClock(Start));
        registry.GetContext("u", "login").SequenceFor("USER", "SAM"); // 1
        registry.GetContext("u", "login").SequenceFor("USER", "BOB"); // 2

        registry.Clear("u", "login");

        Assert.Equal(1, registry.GetContext("u", "login").SequenceFor("USER", "BOB")); // reset
    }

    // REQ-0017: clearing one context leaves another untouched.
    [Fact]
    public void Clear_DoesNotAffectOtherContexts()
    {
        var registry = Registry(new FakeClock(Start));
        registry.GetContext("u1", "login").SequenceFor("USER", "SAM"); // u1: 1
        registry.GetContext("u2", "login").SequenceFor("USER", "SAM"); // u2: 1

        registry.Clear("u1", "login");

        Assert.Equal(2, registry.GetContext("u2", "login").SequenceFor("USER", "BOB")); // u2 intact
    }

    // REQ-0018: a context idle for >= 24h is discarded; the next access starts fresh.
    [Fact]
    public void GetContext_AfterIdleTimeout_IsFresh()
    {
        var clock = new FakeClock(Start);
        var registry = Registry(clock);
        registry.GetContext("u", "login").SequenceFor("USER", "SAM"); // 1

        clock.Advance(TimeSpan.FromHours(25));

        Assert.Equal(1, registry.GetContext("u", "login").SequenceFor("USER", "SAM")); // reset -> new context
    }

    // REQ-0018: a context idle for < 24h is retained.
    [Fact]
    public void GetContext_WithinIdleTimeout_IsRetained()
    {
        var clock = new FakeClock(Start);
        var registry = Registry(clock);
        registry.GetContext("u", "login").SequenceFor("USER", "SAM"); // 1

        clock.Advance(TimeSpan.FromHours(23));

        Assert.Equal(2, registry.GetContext("u", "login").SequenceFor("USER", "BOB")); // same context
    }

    // REQ-0018: an idle context is purged even when a different key is accessed.
    [Fact]
    public void AccessingOneKey_PurgesAnotherExpiredKey()
    {
        var clock = new FakeClock(Start);
        var registry = Registry(clock);
        registry.GetContext("stale", "login");
        Assert.Equal(1, registry.Count);

        clock.Advance(TimeSpan.FromHours(25));
        registry.GetContext("fresh", "login"); // triggers purge of "stale"

        Assert.Equal(1, registry.Count); // only "fresh" remains
    }

    [Fact]
    public void GetContext_BlankArguments_AreRejected()
    {
        var registry = Registry(new FakeClock(Start));

        Assert.Throws<ArgumentException>(() => registry.GetContext("", "login"));
        Assert.Throws<ArgumentException>(() => registry.GetContext("u", ""));
    }

    [Fact]
    public void Registry_NullDependencies_AreRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new PseudonymContextRegistry(null!, new FakeClock(Start)));
        Assert.Throws<ArgumentNullException>(() => new PseudonymContextRegistry(Sha256, null!));
    }
}
