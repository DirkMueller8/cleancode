namespace Logger.Core;

/// <summary>
/// The filter for <c>nonsensitive</c> fields (REQ-0007): copies the value unchanged into the filtered
/// view — nonsensitive data carries no privacy risk and needs no wrapping. A pure function of the
/// input's value; it ignores the field name and pseudonym context in the <see cref="FilterInput"/>.
/// </summary>
public sealed class NonsensitiveFilter : IFieldFilter
{
    public string Apply(FilterInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Value;
    }
}
