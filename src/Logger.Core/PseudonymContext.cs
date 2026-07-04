namespace Logger.Core;

/// <summary>
/// Default <see cref="IPseudonymContext"/> (REQ-0016/0009/0010). Assigns stable, per-prefix sequence
/// numbers, keyed by a salted digest of the value so the <b>raw value is never retained</b> (only its
/// digest is stored — REQ-0010, ISO/IEC 24772-1 [MVX]/[XYL]). One instance per (user, log) view; the
/// salt is supplied per context so identical values in different contexts need not share a digest.
/// </summary>
public sealed class PseudonymContext : IPseudonymContext
{
    private readonly IHasher hasher;
    private readonly string salt;
    private readonly Dictionary<string, PrefixSequence> byPrefix = new(StringComparer.Ordinal);

    public PseudonymContext(IHasher hasher, string salt)
    {
        ArgumentNullException.ThrowIfNull(hasher);
        ArgumentNullException.ThrowIfNull(salt);
        this.hasher = hasher;
        this.salt = salt;
    }

    public int SequenceFor(string prefix, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
        ArgumentNullException.ThrowIfNull(value);

        if (!this.byPrefix.TryGetValue(prefix, out PrefixSequence? sequence))
        {
            sequence = new PrefixSequence();
            this.byPrefix.Add(prefix, sequence);
        }

        string digest = this.hasher.Digest(value, this.salt);
        return sequence.NumberFor(digest);
    }

    /// <summary>Per-prefix allocation of sequence numbers, keyed by value digest (never the raw value).</summary>
    private sealed class PrefixSequence
    {
        private readonly Dictionary<string, int> byDigest = new(StringComparer.Ordinal);
        private int next;

        public int NumberFor(string digest)
        {
            if (this.byDigest.TryGetValue(digest, out int existing))
            {
                return existing;
            }

            int assigned = ++this.next;
            this.byDigest.Add(digest, assigned);
            return assigned;
        }
    }
}
