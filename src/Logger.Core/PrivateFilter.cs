namespace Logger.Core;

/// <summary>
/// The filter for <c>private</c> fields (REQ-0008): replaces the value with a pseudonym of the form
/// <c>{prefix}{n}({length})</c> — e.g. <c>USER1(3)</c>, <c>PW1(12)</c>. The prefix comes from the field
/// via an injected <see cref="IPrefixPolicy"/>; the sequence number <c>n</c> comes from the
/// <see cref="IPseudonymContext"/> (stable per value within a context, REQ-0009); the hint is the raw
/// value's length. The raw value never appears in the result.
/// </summary>
public sealed class PrivateFilter : IFieldFilter
{
    private readonly IPrefixPolicy prefixPolicy;

    public PrivateFilter(IPrefixPolicy prefixPolicy)
    {
        ArgumentNullException.ThrowIfNull(prefixPolicy);
        this.prefixPolicy = prefixPolicy;
    }

    public string Apply(FilterInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        string prefix = this.prefixPolicy.PrefixFor(input.FieldName);
        int sequence = input.Pseudonyms.SequenceFor(prefix, input.Value);
        return $"{prefix}{sequence}({input.Value.Length})";
    }
}
