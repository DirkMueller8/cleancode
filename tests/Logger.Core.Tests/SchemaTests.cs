using Logger.Core;

namespace Logger.Core.Tests;

/// <summary>
/// Tests for REQ-0002 — a log schema of typed fields. Each test maps to an acceptance criterion.
/// </summary>
public sealed class SchemaTests
{
    private static FieldDefinition Field(string name, FieldType type = FieldType.String) =>
        new(name, type);

    // AC: The supported field types are exactly Time, IpAddress, String, Integer.
    [Fact]
    public void FieldType_SupportedValues_AreExactlyTheFour()
    {
        FieldType[] actual = Enum.GetValues<FieldType>();

        Assert.Equal(
            new[] { FieldType.Time, FieldType.IpAddress, FieldType.String, FieldType.Integer },
            actual);
    }

    // AC: A LogType exposes its fields, each with a name and a type.
    [Fact]
    public void LogType_ExposesItsFields_WithNameAndType()
    {
        var login = new LogType("login", new[]
        {
            Field("timestamp", FieldType.Time),
            Field("ipaddr", FieldType.IpAddress),
            Field("user", FieldType.String),
        });

        Assert.Equal(3, login.Fields.Count);
        Assert.True(login.TryGetField("ipaddr", out FieldDefinition? ipaddr));
        Assert.Equal("ipaddr", ipaddr!.Name);
        Assert.Equal(FieldType.IpAddress, ipaddr.Type);
    }

    // AC: A field name duplicated within one LogType is rejected.
    [Fact]
    public void LogType_WithDuplicateFieldName_IsRejected()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() =>
            new LogType("login", new[] { Field("user"), Field("user") }));

        Assert.Contains("user", ex.Message);
    }

    // AC: A schema can hold multiple LogTypes, each retrievable by name.
    [Fact]
    public void Schema_HoldsMultipleLogTypes_RetrievableByName()
    {
        var login = new LogType("login", new[] { Field("timestamp", FieldType.Time), Field("user") });
        var logout = new LogType("logout", new[] { Field("timestamp", FieldType.Time), Field("user") });

        var schema = new Schema(new[] { login, logout });

        Assert.Equal(2, schema.LogTypes.Count);
        Assert.True(schema.TryGetLogType("login", out LogType? found));
        Assert.Same(login, found);
        Assert.False(schema.TryGetLogType("missing", out _));
    }

    // AC: Two LogTypes with the same name in one schema are rejected.
    [Fact]
    public void Schema_WithDuplicateLogTypeName_IsRejected()
    {
        var first = new LogType("login", new[] { Field("timestamp", FieldType.Time), Field("user") });
        var second = new LogType("login", new[] { Field("timestamp", FieldType.Time), Field("ip") });

        ArgumentException ex = Assert.Throws<ArgumentException>(() =>
            new Schema(new[] { first, second }));

        Assert.Contains("login", ex.Message);
    }

    // AC: Names are compared case-sensitively ("user" and "User" are distinct).
    [Fact]
    public void LogType_TreatsFieldNames_CaseSensitively()
    {
        var login = new LogType("login", new[] { Field("user"), Field("User") });

        Assert.Equal(2, login.Fields.Count);
        Assert.True(login.TryGetField("user", out _));
        Assert.False(login.TryGetField("USER", out _));
    }

    [Fact]
    public void Schema_TreatsLogTypeNames_CaseSensitively()
    {
        var lower = new LogType("login", new[] { Field("timestamp", FieldType.Time), Field("user") });
        var upper = new LogType("Login", new[] { Field("timestamp", FieldType.Time), Field("user") });

        var schema = new Schema(new[] { lower, upper });

        Assert.Equal(2, schema.LogTypes.Count);
        Assert.True(schema.TryGetLogType("login", out _));
        Assert.False(schema.TryGetLogType("LOGIN", out _));
    }

    // Argument guards (defensive boundaries per code-policy / ISO 24772 [XYH]).
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void FieldDefinition_WithBlankName_IsRejected(string name) =>
        Assert.Throws<ArgumentException>(() => new FieldDefinition(name, FieldType.String));

    [Fact]
    public void FieldDefinition_WithNullName_IsRejected() =>
        Assert.Throws<ArgumentNullException>(() => new FieldDefinition(null!, FieldType.String));

    [Fact]
    public void LogType_WithNullFields_IsRejected() =>
        Assert.Throws<ArgumentNullException>(() => new LogType("login", null!));
}
