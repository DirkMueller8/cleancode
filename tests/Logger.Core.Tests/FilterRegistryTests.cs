using Logger.Core;
using static Logger.Core.Tests.Build;

namespace Logger.Core.Tests;

/// <summary>
/// Tests for REQ-0013 — pluggable custom filters (Open/Closed). Adding a filter is a new class plus a
/// registration call, with no change to existing filters or the assembler.
/// </summary>
public sealed class FilterRegistryTests
{
    // AC (the OCP headline): a brand-new filter class, registered under a new name, resolves and applies.
    // Nothing in Logger.Core changed to support "shout" — only this new class + the Register call below.
    [Fact]
    public void Register_NewCustomFilter_IsResolvedAndApplied()
    {
        var registry = new FilterRegistry();
        registry.Register("shout", new ShoutFilter());

        Assert.True(registry.IsRegistered("shout"));
        IFieldFilter resolved = registry.Resolve("shout");
        Assert.Equal("HELLO", resolved.Apply(Input("msg", "hello", PseudoContext())));
    }

    // AC: built-ins are registered the same way — no special-casing. The default registry resolves
    // "private" to a filter that produces the same pseudonym a PrivateFilter would.
    [Fact]
    public void BuiltInFilters_AreResolvedThroughTheSamePath()
    {
        IFieldFilter privateFilter = DefaultRegistry.Resolve(Disposition.PrivateName);

        Assert.Equal("USER1(3)", privateFilter.Apply(Input("user", "SAM", PseudoContext())));
    }

    // AC: registering a duplicate disposition name is rejected (no silent override).
    [Fact]
    public void Register_DuplicateDisposition_IsRejected()
    {
        var registry = new FilterRegistry();
        registry.Register("shout", new ShoutFilter());

        Assert.Throws<ArgumentException>(() => registry.Register("shout", new ShoutFilter()));
    }

    [Fact]
    public void Resolve_UnknownDisposition_IsRejected() =>
        Assert.Throws<ArgumentException>(() => new FilterRegistry().Resolve("nope"));

    // AC: an unknown disposition is rejected at schema validation; registering it first makes it valid.
    [Fact]
    public void Register_ThenSchemaWithThatDisposition_IsAccepted()
    {
        var registry = new FilterRegistry();
        registry.Register(Disposition.NonsensitiveName, new NonsensitiveFilter());
        registry.Register("shout", new ShoutFilter());

        var login = new LogType("login", new[]
        {
            Field("timestamp", FieldType.Time),
            Field("msg", FieldType.String, new Disposition("shout")),
        });

        var schema = new Schema(new[] { login }, registry);

        Assert.True(schema.TryGetLogType("login", out _));
    }

    [Fact]
    public void Register_NullFilter_IsRejected() =>
        Assert.Throws<ArgumentNullException>(() => new FilterRegistry().Register("x", null!));

    /// <summary>A brand-new filter, defined only in the test — proof the engine is open for extension.</summary>
    private sealed class ShoutFilter : IFieldFilter
    {
        public string Apply(FilterInput input) => input.Value.ToUpperInvariant();
    }
}
