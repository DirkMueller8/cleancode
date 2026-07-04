using Logger.Core;
using static Logger.Services.Tests.Build;

namespace Logger.Services.Tests;

/// <summary>Tests for the append-only store (REQ-0025) and the secure token generator (REQ-0023).</summary>
public sealed class StoreAndTokenTests
{
    // REQ-0025: appended events are readable in append order.
    [Fact]
    public void Store_Append_ThenAll_ReturnsInOrder()
    {
        var store = new InMemoryLogStore();
        LogEvent first = Event(("timestamp", "1"), ("user", "A"));
        LogEvent second = Event(("timestamp", "2"), ("user", "B"));

        store.Append(first);
        store.Append(second);

        Assert.Equal(new[] { first, second }, store.All());
    }

    // REQ-0025: All() returns a snapshot — mutating it doesn't affect the store.
    [Fact]
    public void Store_All_IsASnapshot()
    {
        var store = new InMemoryLogStore();
        store.Append(ValidEvent());

        IReadOnlyList<LogEvent> snapshot = store.All();
        store.Append(ValidEvent());

        Assert.Single(snapshot);      // the earlier snapshot is unchanged
        Assert.Equal(2, store.All().Count);
    }

    [Fact]
    public void Store_AppendNull_IsRejected() =>
        Assert.Throws<ArgumentNullException>(() => new InMemoryLogStore().Append(null!));

    // REQ-0023: the secure token carries at least 120 bits (>= 30 hex chars) and is random.
    [Fact]
    public void SecureToken_HasAtLeast120Bits_AndIsUnique()
    {
        var generator = new SecureTokenGenerator();

        string a = generator.NewToken();
        string b = generator.NewToken();

        Assert.True(a.Length >= 30, $"token too short: {a.Length} hex chars");
        Assert.NotEqual(a, b);
    }
}
