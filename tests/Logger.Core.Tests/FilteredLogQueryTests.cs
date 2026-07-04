using Logger.Core;

namespace Logger.Core.Tests;

/// <summary>
/// Tests for querying filtered logs by symbol (REQ-0019) and refusing raw-value guesses (REQ-0020).
/// </summary>
public sealed class FilteredLogQueryTests
{
    // "ipaddr" and "user" are pseudonymized; "http" is nonsensitive.
    private static readonly FilteredLogQueryEngine Engine = new(new[] { "ipaddr", "user" });

    private static readonly IReadOnlyList<FilteredView> Entries = new[]
    {
        Entry(("ipaddr", "US1(v4)"), ("http", "POST"), ("user", "USER1(3)")),
        Entry(("ipaddr", "US2(v4)"), ("http", "GET"), ("user", "USER1(3)")),
    };

    // AC (0019): a symbol query matches the entry with that identifier; the hint is ignored.
    [Fact]
    public void Search_BySymbol_MatchesEntriesWithThatIdentifier()
    {
        IReadOnlyList<FilteredView> result = Engine.Search(Entries, Conditions(("ipaddr", "US1")));

        FilteredView match = Assert.Single(result);
        Assert.True(match.TryGetValue("http", out string? http));
        Assert.Equal("POST", http);
    }

    // AC (0019): a non-matching identifier returns nothing.
    [Fact]
    public void Search_ForUnusedSymbol_ReturnsNoEntries()
    {
        Assert.Empty(Engine.Search(Entries, Conditions(("ipaddr", "US9"))));
    }

    // AC (0019): multiple conditions are combined with AND (across a pseudonym and a nonsensitive field).
    [Fact]
    public void Search_MultipleConditions_AreAnded()
    {
        Assert.Empty(Engine.Search(Entries, Conditions(("ipaddr", "US1"), ("http", "GET"))));
        Assert.Single(Engine.Search(Entries, Conditions(("ipaddr", "US1"), ("http", "POST"))));
    }

    // AC (0019): a shared symbol correlates across entries (both share USER1).
    [Fact]
    public void Search_SharedSymbol_CorrelatesAcrossEntries()
    {
        Assert.Equal(2, Engine.Search(Entries, Conditions(("user", "USER1"))).Count);
    }

    // AC (0020): a raw/guessed value on a pseudonymized field is rejected — the inference attack.
    [Fact]
    public void Search_RawValueOnPseudonymField_IsRejected()
    {
        Assert.Throws<ArgumentException>(() =>
            Engine.Search(Entries, Conditions(("ipaddr", "1.1.1.1"))));
    }

    // AC (0020): a valid symbol on a pseudonymized field is allowed.
    [Fact]
    public void Search_SymbolOnPseudonymField_IsAllowed()
    {
        Assert.Single(Engine.Search(Entries, Conditions(("ipaddr", "US1"))));
    }

    // AC (0020): a non-symbol exact value on a nonsensitive field is allowed.
    [Fact]
    public void Search_ExactValueOnNonsensitiveField_IsAllowed()
    {
        Assert.Single(Engine.Search(Entries, Conditions(("http", "POST"))));
    }

    private static FilteredView Entry(params (string Name, string Value)[] fields) =>
        new(fields.Select(f => new KeyValuePair<string, string>(f.Name, f.Value)).ToList());

    private static IReadOnlyList<QueryCondition> Conditions(params (string Field, string Value)[] conditions) =>
        conditions.Select(c => new QueryCondition(c.Field, c.Value)).ToList();
}
