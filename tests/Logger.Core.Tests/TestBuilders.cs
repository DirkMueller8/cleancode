using Logger.Core;

namespace Logger.Core.Tests;

/// <summary>Small builders to keep the tests readable.</summary>
internal static class Build
{
    public static readonly IHasher Sha256 = new Sha256Hasher();

    public static readonly IGeoLookup Geo = new FakeGeoLookup("US");

    /// <summary>A real registry wired with the four built-in filters (REQ-0013).</summary>
    public static readonly IFilterRegistry DefaultRegistry = BuildDefaultRegistry();

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

    public static PseudonymContext PseudoContext(string salt = "salt-A") => new(Sha256, salt);

    public static FilterInput Input(string fieldName, string value, IPseudonymContext pseudonyms) =>
        new(fieldName, value, pseudonyms);

    /// <summary>A registry that merely knows a set of disposition names (filter behaviour irrelevant).</summary>
    public static FilterRegistry RegistryKnowing(params string[] dispositionNames)
    {
        var registry = new FilterRegistry();
        foreach (string name in dispositionNames)
        {
            registry.Register(name, new NonsensitiveFilter());
        }

        return registry;
    }

    private static FilterRegistry BuildDefaultRegistry()
    {
        var registry = new FilterRegistry();
        registry.Register(Disposition.NonsensitiveName, new NonsensitiveFilter());
        registry.Register(Disposition.PrivateName, new PrivateFilter(new DefaultPrefixPolicy()));
        registry.Register("minute", new MinuteFilter());
        registry.Register("country", new CountryFilter(Geo));
        return registry;
    }
}

/// <summary>A stub geo lookup that reports a fixed country for any address.</summary>
internal sealed class FakeGeoLookup : IGeoLookup
{
    private readonly string country;

    public FakeGeoLookup(string country) => this.country = country;

    public string CountryOf(string ipAddress) => this.country;
}

/// <summary>A controllable clock: tests set the start time and advance it deterministically.</summary>
internal sealed class FakeClock : IClock
{
    public FakeClock(DateTimeOffset start) => this.UtcNow = start;

    public DateTimeOffset UtcNow { get; private set; }

    public void Advance(TimeSpan by) => this.UtcNow += by;
}
