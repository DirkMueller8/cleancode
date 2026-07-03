namespace Logger.Core;

/// <summary>
/// Knows which disposition names have a registered filter. REQ-0004 depends only on the membership
/// query below, to validate a schema at build time. Registration of filters and resolution of the
/// actual <c>IFieldFilter</c> for a disposition are added by REQ-0013 (the interface will grow then).
/// Kept minimal here on purpose (ISP): the schema only needs to ask "is this name known?".
/// </summary>
public interface IFilterRegistry
{
    /// <summary>
    /// True if a filter is registered under the given disposition name. Comparison is case-sensitive,
    /// consistent with the rest of the model.
    /// </summary>
    bool IsRegistered(string dispositionName);
}
