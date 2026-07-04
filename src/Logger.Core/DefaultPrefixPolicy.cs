namespace Logger.Core;

/// <summary>
/// Default <see cref="IPrefixPolicy"/> (REQ-0008): a small map of well-known field names to short
/// prefixes (<c>user</c> → <c>USER</c>, <c>password</c> → <c>PW</c>), falling back to the uppercased
/// field name for anything else. Names are matched case-sensitively (glossary → <i>name</i>).
/// </summary>
public sealed class DefaultPrefixPolicy : IPrefixPolicy
{
    private static readonly IReadOnlyDictionary<string, string> KnownPrefixes =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["user"] = "USER",
            ["password"] = "PW",
        };

    public string PrefixFor(string fieldName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);

        return KnownPrefixes.TryGetValue(fieldName, out string? prefix)
            ? prefix
            : fieldName.ToUpperInvariant();
    }
}
