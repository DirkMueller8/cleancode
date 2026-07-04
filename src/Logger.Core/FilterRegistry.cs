namespace Logger.Core;

/// <summary>
/// Maps disposition names to the <see cref="IFieldFilter"/> that handles them (REQ-0013). This is the
/// Open/Closed seam of the filtering engine: a new disposition is supported by writing a new
/// <see cref="IFieldFilter"/> class and registering it here — <b>no existing filter or the assembler
/// changes</b>. Built-in filters (nonsensitive, private, minute, country) are registered exactly like
/// custom ones — there is no special-casing. Registering a name twice is rejected, so a filter can't
/// be silently overridden.
/// </summary>
public sealed class FilterRegistry : IFilterRegistry
{
    private readonly Dictionary<string, IFieldFilter> filtersByDisposition = new(StringComparer.Ordinal);

    /// <summary>Registers <paramref name="filter"/> under <paramref name="dispositionName"/>; rejects a duplicate name.</summary>
    public void Register(string dispositionName, IFieldFilter filter)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dispositionName);
        ArgumentNullException.ThrowIfNull(filter);

        if (!this.filtersByDisposition.TryAdd(dispositionName, filter))
        {
            throw new ArgumentException(
                $"A filter is already registered for disposition '{dispositionName}'.",
                nameof(dispositionName));
        }
    }

    public bool IsRegistered(string dispositionName)
    {
        ArgumentNullException.ThrowIfNull(dispositionName);
        return this.filtersByDisposition.ContainsKey(dispositionName);
    }

    public IFieldFilter Resolve(string dispositionName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dispositionName);

        if (!this.filtersByDisposition.TryGetValue(dispositionName, out IFieldFilter? filter))
        {
            throw new ArgumentException(
                $"No filter is registered for disposition '{dispositionName}'.",
                nameof(dispositionName));
        }

        return filter;
    }
}
