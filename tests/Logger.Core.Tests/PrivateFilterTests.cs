using Logger.Core;
using static Logger.Core.Tests.Build;

namespace Logger.Core.Tests;

/// <summary>
/// Tests for REQ-0008 — private fields become pseudonyms with a format/length hint.
/// </summary>
public sealed class PrivateFilterTests
{
    private static readonly IFieldFilter Filter = new PrivateFilter(new DefaultPrefixPolicy());

    // AC: A 3-char username -> USER1(3); a 12-char password -> PW1(12).
    [Fact]
    public void Apply_WrapsPrivateValues_WithPrefixNumberAndLengthHint()
    {
        PseudonymContext ctx = PseudoContext();

        Assert.Equal("USER1(3)", Filter.Apply(Input("user", "SAM", ctx)));
        Assert.Equal("PW1(12)", Filter.Apply(Input("password", "123456789012", ctx)));
    }

    // AC: The raw value never appears in the filtered output.
    [Fact]
    public void Apply_DoesNotRevealRawValue()
    {
        string result = Filter.Apply(Input("user", "SAM", PseudoContext()));

        Assert.DoesNotContain("SAM", result);
    }

    // AC: The type prefix is derived from the field; unknown fields fall back to the uppercased name.
    [Fact]
    public void Apply_UsesFallbackPrefix_ForUnmappedField()
    {
        string result = Filter.Apply(Input("ssn", "123", PseudoContext()));

        Assert.StartsWith("SSN", result); // "ssn" -> "SSN"
    }

    // REQ-0009 via the filter: the same value yields the same pseudonym within a context.
    [Fact]
    public void Apply_SameValueInContext_YieldsSamePseudonym()
    {
        PseudonymContext ctx = PseudoContext();

        Assert.Equal(
            Filter.Apply(Input("user", "SAM", ctx)),
            Filter.Apply(Input("user", "SAM", ctx)));
    }

    [Fact]
    public void Apply_NullInput_IsRejected() =>
        Assert.Throws<ArgumentNullException>(() => Filter.Apply(null!));
}
