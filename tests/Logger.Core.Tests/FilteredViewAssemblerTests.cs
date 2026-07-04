using Logger.Core;
using static Logger.Core.Tests.Build;

namespace Logger.Core.Tests;

/// <summary>
/// Tests for REQ-0014 — assembling a full filtered view of an event.
/// </summary>
public sealed class FilteredViewAssemblerTests
{
    private static readonly FilteredViewAssembler Assembler = new(DefaultRegistry);

    // AC (the headline): the DSS worked example, end to end. Each field is transformed by its
    // disposition's filter; the raw example produces exactly the filtered example.
    [Fact]
    public void Assemble_ProducesTheFilteredView_ForTheWholeEvent()
    {
        var login = new LogType("login", new[]
        {
            Field("timestamp", FieldType.Time, new Disposition("minute")),
            Field("ipaddr", FieldType.IpAddress, new Disposition("country")),
            Field("http", FieldType.String, Disposition.Nonsensitive),
            Field("url", FieldType.String, Disposition.Nonsensitive),
            Field("user", FieldType.String, Disposition.Private),
            Field("password", FieldType.String, Disposition.Private),
        });
        LogEvent logEvent = Event("login",
            ("timestamp", "150"),           // -> floored to 120
            ("ipaddr", "66.77.88.99"),      // -> US1(v4)
            ("http", "POST"),               // -> POST
            ("url", "login.html"),          // -> login.html
            ("user", "SAM"),                // -> USER1(3)
            ("password", "123456789012"));  // -> PW1(12)

        FilteredView view = Assembler.Assemble(login, logEvent, PseudoContext());

        AssertField(view, "timestamp", "120");
        AssertField(view, "ipaddr", "US1(v4)");
        AssertField(view, "http", "POST");
        AssertField(view, "url", "login.html");
        AssertField(view, "user", "USER1(3)");
        AssertField(view, "password", "PW1(12)");
        Assert.DoesNotContain(view.Fields, f => f.Value.Contains("SAM"));
        Assert.DoesNotContain(view.Fields, f => f.Value == "66.77.88.99");
    }

    // AC: field order and names are preserved (schema declared order).
    [Fact]
    public void Assemble_PreservesFieldOrderAndNames()
    {
        var login = new LogType("login", new[]
        {
            Field("timestamp", FieldType.Time),
            Field("user", FieldType.String, Disposition.Private),
            Field("http", FieldType.String, Disposition.Nonsensitive),
        });
        LogEvent logEvent = Event("login", ("timestamp", "150"), ("user", "SAM"), ("http", "POST"));

        FilteredView view = Assembler.Assemble(login, logEvent, PseudoContext());

        Assert.Equal(new[] { "timestamp", "user", "http" }, view.Fields.Select(f => f.Key));
    }

    // AC: the assembler holds no per-disposition logic — a custom filter it's never heard of is applied.
    [Fact]
    public void Assemble_AppliesACustomFilter_WithoutKnowingIt()
    {
        var registry = new FilterRegistry();
        registry.Register(Disposition.NonsensitiveName, new NonsensitiveFilter());
        registry.Register("shout", new ShoutFilter());
        var assembler = new FilteredViewAssembler(registry);
        var login = new LogType("login", new[]
        {
            Field("timestamp", FieldType.Time),
            Field("msg", FieldType.String, new Disposition("shout")),
        });
        LogEvent logEvent = Event("login", ("timestamp", "150"), ("msg", "hello"));

        FilteredView view = assembler.Assemble(login, logEvent, PseudoContext());

        AssertField(view, "msg", "HELLO");
    }

    [Fact]
    public void Assemble_EventMissingADeclaredField_IsRejected()
    {
        var login = new LogType("login", new[]
        {
            Field("timestamp", FieldType.Time),
            Field("user", FieldType.String, Disposition.Private),
        });
        LogEvent logEvent = Event("login", ("timestamp", "150")); // no user

        Assert.Throws<ArgumentException>(() => Assembler.Assemble(login, logEvent, PseudoContext()));
    }

    [Fact]
    public void Assemble_NullArguments_AreRejected()
    {
        var login = new LogType("login", new[] { Field("timestamp", FieldType.Time), Field("user") });
        LogEvent logEvent = Event("login", ("timestamp", "150"), ("user", "x"));

        Assert.Throws<ArgumentNullException>(() => Assembler.Assemble(null!, logEvent, PseudoContext()));
        Assert.Throws<ArgumentNullException>(() => Assembler.Assemble(login, null!, PseudoContext()));
        Assert.Throws<ArgumentNullException>(() => Assembler.Assemble(login, logEvent, null!));
    }

    private static void AssertField(FilteredView view, string name, string expected)
    {
        Assert.True(view.TryGetValue(name, out string? actual));
        Assert.Equal(expected, actual);
    }

    private sealed class ShoutFilter : IFieldFilter
    {
        public string Apply(FilterInput input) => input.Value.ToUpperInvariant();
    }
}
