namespace Logger.Core;

/// <summary>
/// Produces a salted secure digest of a value (REQ-0010). Injected (DIP) so the algorithm is not
/// hard-coded and can be substituted in tests. The salt is supplied per pseudonym context, defeating
/// precomputed-hash (rainbow-table) inference — ISO/IEC 24772-1 [MVX] (one-way hash without a salt).
/// </summary>
public interface IHasher
{
    string Digest(string value, string salt);
}
