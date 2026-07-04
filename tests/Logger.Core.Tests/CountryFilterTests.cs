using Logger.Core;
using static Logger.Core.Tests.Build;

namespace Logger.Core.Tests;

/// <summary>
/// Tests for REQ-0012 — the country filter wraps an IP as {country}{n}({family}), e.g. US1(v4).
/// </summary>
public sealed class CountryFilterTests
{
    private static readonly IFieldFilter Filter = new CountryFilter(new FakeGeoLookup("US"));

    // AC: an IPv4 US address renders as US<n>(v4).
    [Fact]
    public void Apply_WrapsIp_AsCountryPseudonymWithFamilyHint()
    {
        string result = Filter.Apply(Input("ipaddr", "66.77.88.99", PseudoContext()));

        Assert.Equal("US1(v4)", result);
    }

    // AC: two addresses in the same country get the country prefix but distinct sequence numbers.
    [Fact]
    public void Apply_DistinctIps_GetIncreasingNumbers()
    {
        PseudonymContext ctx = PseudoContext();

        Assert.Equal("US1(v4)", Filter.Apply(Input("ipaddr", "66.77.88.99", ctx)));
        Assert.Equal("US2(v4)", Filter.Apply(Input("ipaddr", "10.0.0.1", ctx)));
    }

    // Stability (REQ-0009) via the filter: same IP -> same pseudonym in a context.
    [Fact]
    public void Apply_SameIp_YieldsSamePseudonym()
    {
        PseudonymContext ctx = PseudoContext();

        Assert.Equal(
            Filter.Apply(Input("ipaddr", "66.77.88.99", ctx)),
            Filter.Apply(Input("ipaddr", "66.77.88.99", ctx)));
    }

    // AC: the address-family hint reflects IPv6.
    [Fact]
    public void Apply_IPv6Address_HasV6Hint()
    {
        string result = Filter.Apply(Input("ipaddr", "2001:db8::1", PseudoContext()));

        Assert.Equal("US1(v6)", result);
    }

    [Fact]
    public void Apply_DoesNotRevealRawIp()
    {
        string result = Filter.Apply(Input("ipaddr", "66.77.88.99", PseudoContext()));

        Assert.DoesNotContain("66.77.88.99", result);
    }

    [Fact]
    public void Apply_NullInput_IsRejected() =>
        Assert.Throws<ArgumentNullException>(() => Filter.Apply(null!));
}
