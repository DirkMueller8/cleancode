namespace Logger.Core;

/// <summary>
/// The filter for <c>nonsensitive</c> fields (REQ-0007): copies the value unchanged into the filtered
/// view — nonsensitive data carries no privacy risk and needs no wrapping. A pure function of its input
/// (no state, no context), which is why it's the trivial first concrete <see cref="IFieldFilter"/>.
/// </summary>
public sealed class NonsensitiveFilter : IFieldFilter
{
    public string Apply(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value;
    }
}
