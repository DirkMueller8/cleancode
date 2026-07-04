using Logger.Core;
using static Logger.Core.Tests.Build;

namespace Logger.Core.Tests;

/// <summary>
/// Tests for the pseudonym context: stability (REQ-0009), salted-digest keying with no raw value
/// retained (REQ-0010), and per-context isolation (REQ-0016).
/// </summary>
public sealed class PseudonymContextTests
{
    // REQ-0009: equal values share a number within a context.
    [Fact]
    public void SequenceFor_SameValue_ReturnsSameNumber()
    {
        PseudonymContext ctx = PseudoContext();

        Assert.Equal(ctx.SequenceFor("USER", "SAM"), ctx.SequenceFor("USER", "SAM"));
    }

    // REQ-0009: distinct values get distinct, increasing numbers in first-seen order.
    [Fact]
    public void SequenceFor_DistinctValues_IncreaseInFirstSeenOrder()
    {
        PseudonymContext ctx = PseudoContext();

        Assert.Equal(1, ctx.SequenceFor("USER", "SAM"));
        Assert.Equal(2, ctx.SequenceFor("USER", "BOB"));
        Assert.Equal(1, ctx.SequenceFor("USER", "SAM")); // still 1 — stable
    }

    // REQ-0008/0009: numbering is per prefix (each prefix starts at 1).
    [Fact]
    public void SequenceFor_NumbersAreScopedPerPrefix()
    {
        PseudonymContext ctx = PseudoContext();

        Assert.Equal(1, ctx.SequenceFor("USER", "SAM"));
        Assert.Equal(1, ctx.SequenceFor("PW", "secret")); // its own counter, also starts at 1
    }

    // REQ-0010: the context keys by the injected hasher's digest, not the raw value.
    // A colliding hasher makes two different values map to the same number — impossible unless the
    // digest (not the raw value) is the key.
    [Fact]
    public void SequenceFor_KeysByDigest_NotRawValue()
    {
        var ctx = new PseudonymContext(new CollidingHasher(), "salt");

        Assert.Equal(ctx.SequenceFor("USER", "ALICE"), ctx.SequenceFor("USER", "BOB"));
    }

    // REQ-0010: the digest is salted — the same value under a different salt yields a different digest.
    [Fact]
    public void Sha256Hasher_DifferentSalt_ProducesDifferentDigest()
    {
        var hasher = new Sha256Hasher();

        Assert.NotEqual(hasher.Digest("SAM", "salt-A"), hasher.Digest("SAM", "salt-B"));
    }

    // REQ-0016: separate contexts keep independent mappings.
    [Fact]
    public void Contexts_AreIndependent()
    {
        PseudonymContext a = PseudoContext("salt-A");
        PseudonymContext b = PseudoContext("salt-B");

        a.SequenceFor("USER", "SAM"); // a: SAM -> 1
        int aBob = a.SequenceFor("USER", "BOB"); // a: BOB -> 2
        int bBob = b.SequenceFor("USER", "BOB"); // b: BOB -> 1 (independent)

        Assert.Equal(2, aBob);
        Assert.Equal(1, bBob);
    }

    /// <summary>A hasher that maps every value to the same digest, to prove digest-keying.</summary>
    private sealed class CollidingHasher : IHasher
    {
        public string Digest(string value, string salt) => "SAME";
    }
}
