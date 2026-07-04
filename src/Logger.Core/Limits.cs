namespace Logger.Core;

/// <summary>
/// The single source of truth for input-size limits (REQ-0006, from DSS §5). Bounding inputs at the
/// boundary guards against resource exhaustion (ISO/IEC 24772-1 [XZP]) and unbounded tainted data
/// ([EFS]). Lengths are counted in C# <see cref="string.Length"/> — UTF-16 code units — which equals
/// the Unicode character count except for astral-plane characters (e.g. emoji); acceptable at this
/// coarse limit.
/// </summary>
public static class Limits
{
    /// <summary>Maximum length of a whole request (the raw request text).</summary>
    public const int MaxRequestChars = 1_000_000;

    /// <summary>Maximum length of any individual field value.</summary>
    public const int MaxFieldValueChars = 10_000;
}
