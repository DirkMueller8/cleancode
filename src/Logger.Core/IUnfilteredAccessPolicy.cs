namespace Logger.Core;

/// <summary>
/// Decides whether a caller may see unfiltered (raw) log data (REQ-0015). An injected seam: the real
/// policy — authorization, approvals, tokens (Epic H) — is out of scope for the core and supplied by
/// the host. The core reveals raw data only when the provided policy allows.
/// </summary>
public interface IUnfilteredAccessPolicy
{
    bool IsAllowed(AccessRequest request);
}
