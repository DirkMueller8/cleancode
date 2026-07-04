using Logger.Core;
using static Logger.Core.Tests.Build;

namespace Logger.Core.Tests;

/// <summary>
/// Tests for REQ-0003 — a LogType must declare a timestamp (a field of type Time) and at least one
/// other identifying field. Validated when the schema is built.
/// </summary>
public sealed class TimestampAndIdentifierTests
{
    // AC: A LogType with a timestamp and >= 1 other field is accepted.
    [Fact]
    public void Schema_WithTimestampAndAnotherField_IsAccepted()
    {
        var login = new LogType("login", new[] { Field("timestamp", FieldType.Time), Field("user") });

        Schema schema = SchemaOf(login);

        Assert.True(schema.TryGetLogType("login", out _));
    }

    // AC: A LogType with only a timestamp is rejected; rejection identifies the LogType by name.
    [Fact]
    public void Schema_WithOnlyTimestamp_IsRejected()
    {
        var ping = new LogType("ping", new[] { Field("timestamp", FieldType.Time) });

        ArgumentException ex = Assert.Throws<ArgumentException>(() => SchemaOf(ping));

        Assert.Contains("ping", ex.Message);
    }

    // AC: A LogType with no timestamp is rejected; rejection identifies the LogType by name.
    [Fact]
    public void Schema_WithNoTimestamp_IsRejected()
    {
        var ping = new LogType("ping", new[] { Field("ipaddr", FieldType.IpAddress), Field("user") });

        ArgumentException ex = Assert.Throws<ArgumentException>(() => SchemaOf(ping));

        Assert.Contains("ping", ex.Message);
        Assert.Contains("timestamp", ex.Message);
    }

    // Documented edge (REQ-0003 resolution): a second Time field counts as the identifying field.
    [Fact]
    public void Schema_WithTwoTimeFields_IsAccepted()
    {
        var span = new LogType("span", new[] { Field("start", FieldType.Time), Field("end", FieldType.Time) });

        Schema schema = SchemaOf(span);

        Assert.True(schema.TryGetLogType("span", out _));
    }
}
