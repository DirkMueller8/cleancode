using Logger.Core;
using static Logger.Core.Tests.Build;

namespace Logger.Core.Tests;

/// <summary>
/// Tests for REQ-0006 — request and field size limits. Boundary-value tests are the point.
/// </summary>
public sealed class SizeLimitTests
{
    private static readonly SizeValidator Validator = new();

    // AC: A request of exactly 1,000,000 chars is accepted; 1,000,001 is rejected.
    [Fact]
    public void ValidateRequestSize_AtLimit_IsValid()
    {
        ValidationResult result = Validator.ValidateRequestSize(new string('x', Limits.MaxRequestChars));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateRequestSize_OverLimit_IsInvalid()
    {
        ValidationResult result = Validator.ValidateRequestSize(new string('x', Limits.MaxRequestChars + 1));

        Assert.False(result.IsValid);
        Assert.Contains(result.Violations, v => v.Contains("Request length"));
    }

    // AC: A field value of exactly 10,000 chars is accepted; 10,001 is rejected.
    [Fact]
    public void ValidateFieldSizes_AtLimit_IsValid()
    {
        LogEvent logEvent = Event("login", ("user", new string('x', Limits.MaxFieldValueChars)));

        ValidationResult result = Validator.ValidateFieldSizes(logEvent);

        Assert.True(result.IsValid);
    }

    // AC: The rejection states which limit was exceeded and the field.
    [Fact]
    public void ValidateFieldSizes_OverLimit_IsInvalid_NamingField()
    {
        LogEvent logEvent = Event("login", ("user", new string('x', Limits.MaxFieldValueChars + 1)));

        ValidationResult result = Validator.ValidateFieldSizes(logEvent);

        Assert.False(result.IsValid);
        Assert.Contains(result.Violations, v => v.Contains("user"));
    }

    [Fact]
    public void ValidateRequestSize_NullRequest_IsRejected() =>
        Assert.Throws<ArgumentNullException>(() => Validator.ValidateRequestSize(null!));

    [Fact]
    public void ValidateFieldSizes_NullEvent_IsRejected() =>
        Assert.Throws<ArgumentNullException>(() => Validator.ValidateFieldSizes(null!));
}
