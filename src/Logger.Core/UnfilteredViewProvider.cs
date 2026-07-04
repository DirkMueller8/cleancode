namespace Logger.Core;

/// <summary>
/// Returns the unfiltered (raw) values of an event to an authorized caller, and denies everyone else
/// (REQ-0015). <b>Deny by default:</b> values are revealed only when the injected
/// <see cref="IUnfilteredAccessPolicy"/> allows the request. Kept separate from filtering (SRP).
/// Auditing the fact that unfiltered access occurred is deferred to Epic H (REQ-0035).
/// </summary>
public sealed class UnfilteredViewProvider
{
    private readonly IUnfilteredAccessPolicy policy;

    public UnfilteredViewProvider(IUnfilteredAccessPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(policy);
        this.policy = policy;
    }

    public UnfilteredAccessResult Reveal(LogEvent logEvent, AccessRequest request)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(request);

        return this.policy.IsAllowed(request)
            ? UnfilteredAccessResult.Granted(logEvent.Values)
            : UnfilteredAccessResult.Denied;
    }
}
