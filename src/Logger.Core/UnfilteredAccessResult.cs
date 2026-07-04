namespace Logger.Core;

/// <summary>
/// The outcome of an unfiltered-access request (REQ-0015). A denied result carries <b>no</b> values,
/// and reading <see cref="Values"/> on it throws — so raw private data cannot leak from a denial, even
/// by a caller that forgets to check <see cref="IsGranted"/> (ISO/IEC 24772-1 [OYB]).
/// </summary>
public sealed class UnfilteredAccessResult
{
    private readonly IReadOnlyDictionary<string, string>? values;

    public bool IsGranted { get; }

    private UnfilteredAccessResult(bool isGranted, IReadOnlyDictionary<string, string>? values)
    {
        this.IsGranted = isGranted;
        this.values = values;
    }

    public static UnfilteredAccessResult Granted(IReadOnlyDictionary<string, string> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new UnfilteredAccessResult(true, values);
    }

    public static UnfilteredAccessResult Denied { get; } = new(false, null);

    /// <summary>The raw values — available only when <see cref="IsGranted"/> is true; throws otherwise.</summary>
    public IReadOnlyDictionary<string, string> Values =>
        this.IsGranted
            ? this.values!
            : throw new InvalidOperationException("Unfiltered access was denied; no raw values are available.");
}
