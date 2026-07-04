using Logger.Core;
using static Logger.Core.Tests.Build;

namespace Logger.Core.Tests;

/// <summary>
/// Tests for REQ-0015 — the unfiltered (raw) view is returned only to an authorized caller, and a
/// denial leaks nothing.
/// </summary>
public sealed class UnfilteredViewTests
{
    private static LogEvent SampleEvent() => Event("login",
        ("timestamp", "150"),
        ("user", "SAM"),
        ("password", "secret123"));

    private static AccessRequest Request() => new("investigator", "login");

    // AC: an authorized caller receives the exact raw values (not the wrapped ones).
    [Fact]
    public void Reveal_WhenAuthorized_ReturnsExactRawValues()
    {
        var provider = new UnfilteredViewProvider(new AllowAllPolicy());

        UnfilteredAccessResult result = provider.Reveal(SampleEvent(), Request());

        Assert.True(result.IsGranted);
        Assert.Equal("SAM", result.Values["user"]);
        Assert.Equal("secret123", result.Values["password"]); // raw, not PW1(9)
    }

    // AC: an unauthorized caller is denied and receives no raw values (reading them throws).
    [Fact]
    public void Reveal_WhenUnauthorized_IsDenied_AndLeaksNoValues()
    {
        var provider = new UnfilteredViewProvider(new DenyAllPolicy());

        UnfilteredAccessResult result = provider.Reveal(SampleEvent(), Request());

        Assert.False(result.IsGranted);
        Assert.Throws<InvalidOperationException>(() => result.Values);
    }

    // AC: the authorization decision is made through the injected policy, not hard-coded.
    [Fact]
    public void Reveal_UsesInjectedPolicy_NotHardCoded()
    {
        LogEvent logEvent = SampleEvent();
        AccessRequest request = Request();

        Assert.True(new UnfilteredViewProvider(new AllowAllPolicy()).Reveal(logEvent, request).IsGranted);
        Assert.False(new UnfilteredViewProvider(new DenyAllPolicy()).Reveal(logEvent, request).IsGranted);
    }

    [Fact]
    public void Reveal_NullArguments_AreRejected()
    {
        var provider = new UnfilteredViewProvider(new AllowAllPolicy());

        Assert.Throws<ArgumentNullException>(() => provider.Reveal(null!, Request()));
        Assert.Throws<ArgumentNullException>(() => provider.Reveal(SampleEvent(), null!));
    }

    [Fact]
    public void Provider_NullPolicy_IsRejected() =>
        Assert.Throws<ArgumentNullException>(() => new UnfilteredViewProvider(null!));

    private sealed class AllowAllPolicy : IUnfilteredAccessPolicy
    {
        public bool IsAllowed(AccessRequest request) => true;
    }

    private sealed class DenyAllPolicy : IUnfilteredAccessPolicy
    {
        public bool IsAllowed(AccessRequest request) => false;
    }
}
