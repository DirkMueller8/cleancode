using Logger.Core;
using static Logger.Core.Tests.Build;

namespace Logger.Core.Tests;

/// <summary>
/// Tests for REQ-0004 — every field must declare a disposition, validated at schema-build time.
/// </summary>
public sealed class DispositionTests
{
    // AC: A field with no disposition is rejected (a field cannot be built without one).
    [Fact]
    public void FieldDefinition_WithNullDisposition_IsRejected() =>
        Assert.Throws<ArgumentNullException>(() => new FieldDefinition("user", FieldType.String, null!));

    [Fact]
    public void FieldDefinition_ExposesItsDisposition()
    {
        var field = new FieldDefinition("user", FieldType.String, Disposition.Private);

        Assert.Equal(Disposition.PrivateName, field.Disposition.Name);
    }

    // AC: A field with a valid (registered) disposition is accepted.
    [Fact]
    public void Schema_WithRegisteredDisposition_IsAccepted()
    {
        var login = new LogType("login", new[]
        {
            Field("timestamp", FieldType.Time),
            Field("user", FieldType.String, Disposition.Private),
        });

        Schema schema = SchemaOf(login); // DefaultRegistry knows "nonsensitive" and "private"

        Assert.True(schema.TryGetLogType("login", out _));
    }

    // AC: A field naming an unknown disposition is rejected, identifying the offending field.
    [Fact]
    public void Schema_WithUnknownDisposition_IsRejected_IdentifyingField()
    {
        var registry = new FakeFilterRegistry(Disposition.NonsensitiveName); // does NOT know "secret"
        var login = new LogType("login", new[]
        {
            Field("timestamp", FieldType.Time),
            Field("ssn", FieldType.String, new Disposition("secret")),
        });

        ArgumentException ex = Assert.Throws<ArgumentException>(() => SchemaOf(registry, login));

        Assert.Contains("secret", ex.Message);  // the unknown disposition
        Assert.Contains("ssn", ex.Message);     // the offending field
    }

    [Fact]
    public void Schema_WithNullRegistry_IsRejected()
    {
        var login = new LogType("login", new[] { Field("timestamp", FieldType.Time), Field("user") });

        Assert.Throws<ArgumentNullException>(() => new Schema(new[] { login }, null!));
    }
}
