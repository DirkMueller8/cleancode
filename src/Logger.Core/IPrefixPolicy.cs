namespace Logger.Core;

/// <summary>
/// Decides the pseudonym prefix for a field (REQ-0008), e.g. <c>user</c> → <c>USER</c>. A replaceable
/// policy (the abstraction is the extension seam) so the mapping can be overridden without touching
/// the filter.
/// </summary>
public interface IPrefixPolicy
{
    string PrefixFor(string fieldName);
}
