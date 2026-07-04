using Logger.Core;

namespace Logger.Core.Tests;

/// <summary>
/// A test double for <see cref="IFilterRegistry"/> that knows a fixed set of disposition names.
/// Lets schema tests validate dispositions without any real filters (which arrive in Epic B).
/// </summary>
internal sealed class FakeFilterRegistry : IFilterRegistry
{
    private readonly HashSet<string> known;

    public FakeFilterRegistry(params string[] knownNames) =>
        this.known = new HashSet<string>(knownNames, StringComparer.Ordinal);

    public bool IsRegistered(string dispositionName) => this.known.Contains(dispositionName);
}

/// <summary>Small builders to keep the tests readable.</summary>
internal static class Build
{
    /// <summary>A registry that knows the built-in disposition names used across the tests.</summary>
    public static readonly IFilterRegistry DefaultRegistry =
        new FakeFilterRegistry(
            Disposition.NonsensitiveName,
            Disposition.PrivateName,
            "minute",
            "country");

    public static FieldDefinition Field(
        string name,
        FieldType type = FieldType.String,
        Disposition? disposition = null) =>
        new(name, type, disposition ?? Disposition.Nonsensitive);

    public static Schema SchemaOf(params LogType[] logTypes) => new(logTypes, DefaultRegistry);

    public static Schema SchemaOf(IFilterRegistry registry, params LogType[] logTypes) =>
        new(logTypes, registry);

    public static LogEvent Event(string logTypeName, params (string Name, string Value)[] values) =>
        new(logTypeName, values.ToDictionary(v => v.Name, v => v.Value, StringComparer.Ordinal));

    public static readonly IHasher Sha256 = new Sha256Hasher();

    public static PseudonymContext PseudoContext(string salt = "salt-A") => new(Sha256, salt);

    public static FilterInput Input(string fieldName, string value, IPseudonymContext pseudonyms) =>
        new(fieldName, value, pseudonyms);
}
