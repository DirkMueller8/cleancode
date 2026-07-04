namespace Logger.Core;

/// <summary>
/// Shared formatting for pseudonyms so every filter renders the same shape — <c>{prefix}{n}({hint})</c>
/// (e.g. <c>USER1(3)</c>, <c>US1(v4)</c>). Centralised (DRY, per the REQ-0012 resolution) so
/// <see cref="PrivateFilter"/> and <see cref="CountryFilter"/> cannot drift apart.
/// </summary>
internal static class Pseudonym
{
    public static string Format(string prefix, int sequence, string hint) =>
        $"{prefix}{sequence}({hint})";
}
