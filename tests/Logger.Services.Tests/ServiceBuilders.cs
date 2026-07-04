using Logger.Core;

namespace Logger.Services.Tests;

/// <summary>Small builders for the service-stub tests.</summary>
internal static class Build
{
    /// <summary>A "login" schema with a timestamp + a nonsensitive user field.</summary>
    public static Schema LoginSchema()
    {
        var registry = new FilterRegistry();
        registry.Register(Disposition.NonsensitiveName, new NonsensitiveFilter());

        var login = new LogType("login", new[]
        {
            new FieldDefinition("timestamp", FieldType.Time, Disposition.Nonsensitive),
            new FieldDefinition("user", FieldType.String, Disposition.Nonsensitive),
        });

        return new Schema(new[] { login }, registry);
    }

    public static LogEvent ValidEvent() => Event(("timestamp", "150"), ("user", "SAM"));

    public static LogEvent InvalidEvent() => Event(("timestamp", "150")); // missing "user"

    public static LogEvent Event(params (string Name, string Value)[] fields) =>
        new("login", fields.ToDictionary(f => f.Name, f => f.Value, StringComparer.Ordinal));
}

/// <summary>A token generator returning a fixed token, for deterministic lifecycle tests.</summary>
internal sealed class StubTokenGenerator : ITokenGenerator
{
    private readonly string token;

    public StubTokenGenerator(string token = "TEST-TOKEN") => this.token = token;

    public string NewToken() => this.token;
}
