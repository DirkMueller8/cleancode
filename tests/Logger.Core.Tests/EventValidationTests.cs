using Logger.Core;
using static Logger.Core.Tests.Build;

namespace Logger.Core.Tests;

/// <summary>
/// Tests for REQ-0005 — an event is accepted only if it matches its LogType schema.
/// </summary>
public sealed class EventValidationTests
{
    private static SchemaValidator LoginValidator()
    {
        var login = new LogType("login", new[]
        {
            Field("timestamp", FieldType.Time),
            Field("ipaddr", FieldType.IpAddress),
            Field("user", FieldType.String),
        });
        return new SchemaValidator(SchemaOf(login));
    }

    // AC: An event whose fields match the LogType (names + types) is accepted.
    [Fact]
    public void Validate_MatchingEvent_IsValid()
    {
        ValidationResult result = LoginValidator().Validate(Event("login",
            ("timestamp", "1234567890"),
            ("ipaddr", "12.34.56.78"),
            ("user", "SAM")));

        Assert.True(result.IsValid);
        Assert.Empty(result.Violations);
    }

    // AC: An event missing a declared field is rejected.
    [Fact]
    public void Validate_MissingField_IsInvalid()
    {
        ValidationResult result = LoginValidator().Validate(Event("login",
            ("timestamp", "1234567890"),
            ("user", "SAM")));   // no ipaddr

        Assert.False(result.IsValid);
        Assert.Contains(result.Violations, v => v.Contains("ipaddr"));
    }

    // AC: An event with a field absent from the schema is rejected.
    [Fact]
    public void Validate_UnknownField_IsInvalid()
    {
        ValidationResult result = LoginValidator().Validate(Event("login",
            ("timestamp", "1234567890"),
            ("ipaddr", "12.34.56.78"),
            ("user", "SAM"),
            ("extra", "x")));

        Assert.False(result.IsValid);
        Assert.Contains(result.Violations, v => v.Contains("extra"));
    }

    // AC: An event with a value that doesn't parse to the declared type is rejected.
    [Fact]
    public void Validate_WrongType_IsInvalid()
    {
        ValidationResult result = LoginValidator().Validate(Event("login",
            ("timestamp", "1234567890"),
            ("ipaddr", "not-an-ip"),
            ("user", "SAM")));

        Assert.False(result.IsValid);
        Assert.Contains(result.Violations, v => v.Contains("ipaddr"));
    }

    // AC: Submitting an event for an unknown LogType is rejected.
    [Fact]
    public void Validate_UnknownLogType_IsInvalid()
    {
        ValidationResult result = LoginValidator().Validate(Event("logout",
            ("timestamp", "1234567890")));

        Assert.False(result.IsValid);
        Assert.Contains(result.Violations, v => v.Contains("logout"));
    }

    // The result collects all violations, not just the first (a missing field AND an unknown field).
    [Fact]
    public void Validate_CollectsMultipleViolations()
    {
        ValidationResult result = LoginValidator().Validate(Event("login",
            ("timestamp", "1234567890"),
            ("extra", "x")));   // missing ipaddr + user, plus unknown "extra"

        Assert.False(result.IsValid);
        Assert.True(result.Violations.Count >= 2);
    }

    [Fact]
    public void SchemaValidator_WithNullSchema_IsRejected() =>
        Assert.Throws<ArgumentNullException>(() => new SchemaValidator(null!));

    [Fact]
    public void Validate_NullEvent_IsRejected() =>
        Assert.Throws<ArgumentNullException>(() => LoginValidator().Validate(null!));
}
