using Logger.Core;
using static Logger.Core.Tests.Build;

namespace Logger.Core.Tests;

/// <summary>
/// Tests for REQ-0011 — the minute filter floors an epoch-seconds timestamp to its minute.
/// (Epoch 120 = minute boundary; 179 is within [120,180); 180 starts the next minute.)
/// </summary>
public sealed class MinuteFilterTests
{
    private static readonly IFieldFilter Filter = new MinuteFilter();

    private static string Apply(string epoch) => Filter.Apply(Input("timestamp", epoch, PseudoContext()));

    // AC: seconds are floored away; a value inside a minute maps to that minute's start.
    [Theory]
    [InlineData("120", "120")]   // already on the boundary
    [InlineData("130", "120")]   // 10s in -> floored back
    [InlineData("179", "120")]   // 08:0x:59 analogue -> same minute
    [InlineData("180", "180")]   // 08:10:00 analogue -> next minute
    public void Apply_FloorsToStartOfMinute(string epoch, string expected) =>
        Assert.Equal(expected, Apply(epoch));

    // AC: the unfiltered value is untouched (filtering is pure; the raw string is unchanged).
    [Fact]
    public void Apply_DoesNotMutateInputValue()
    {
        var input = Input("timestamp", "179", PseudoContext());

        string filtered = Filter.Apply(input);

        Assert.Equal("120", filtered);
        Assert.Equal("179", input.Value); // full precision retained on the source
    }

    [Fact]
    public void Apply_NonNumericValue_IsRejected() =>
        Assert.Throws<ArgumentException>(() => Apply("not-a-time"));
}
