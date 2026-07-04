namespace Logger.Core;

/// <summary>
/// The read-only view of the filter registry that consumers depend on. <see cref="Schema"/> uses
/// <see cref="IsRegistered"/> to validate dispositions at build time (REQ-0004); the filtered-view
/// assembler uses <see cref="Resolve"/> to get the filter for a disposition (REQ-0014). Registration
/// is deliberately <b>not</b> here — only the concrete <see cref="FilterRegistry"/> can be mutated,
/// and only at composition time (ISP: query-side clients can't register).
/// </summary>
public interface IFilterRegistry
{
    /// <summary>True if a filter is registered under the given (case-sensitive) disposition name.</summary>
    bool IsRegistered(string dispositionName);

    /// <summary>Returns the filter registered for the disposition; throws if none is registered.</summary>
    IFieldFilter Resolve(string dispositionName);
}
