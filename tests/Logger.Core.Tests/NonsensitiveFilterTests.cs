using Logger.Core;

namespace Logger.Core.Tests;

/// <summary>
/// Tests for REQ-0007 — nonsensitive fields pass through unchanged.
/// </summary>
public sealed class NonsensitiveFilterTests
{
    private static readonly IFieldFilter Filter = new NonsensitiveFilter();

    // AC: A nonsensitive value is identical before and after filtering.
    [Theory]
    [InlineData("POST")]
    [InlineData("login.html")]
    [InlineData("")]
    public void Apply_ReturnsValueUnchanged(string value) =>
        Assert.Equal(value, Filter.Apply(value));

    // AC: Filtering is a pure function of the input (same input -> same output, no side effects).
    [Fact]
    public void Apply_IsPure_SameInputSameOutput()
    {
        Assert.Equal(Filter.Apply("login.html"), Filter.Apply("login.html"));
    }

    [Fact]
    public void Apply_NullValue_IsRejected() =>
        Assert.Throws<ArgumentNullException>(() => Filter.Apply(null!));
}
