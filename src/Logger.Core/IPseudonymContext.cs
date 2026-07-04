namespace Logger.Core;

/// <summary>
/// A per-(user, log) scope within which pseudonyms are stable (REQ-0016). Returns a stable sequence
/// number for a value under a given prefix: within one context, equal values share a number and
/// distinct values get distinct, increasing numbers (REQ-0009). Separate contexts are independent,
/// so one user's view never leaks correlations into another's.
/// </summary>
public interface IPseudonymContext
{
    /// <summary>The stable per-prefix sequence number for <paramref name="value"/> in this context.</summary>
    int SequenceFor(string prefix, string value);
}
