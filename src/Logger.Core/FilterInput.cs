namespace Logger.Core;

/// <summary>
/// The per-call input to an <see cref="IFieldFilter"/> (Epic B): the field being filtered, its raw
/// value, and the <see cref="IPseudonymContext"/> for the current view. Bundled as one parameter
/// object so the interface can gain new per-call data without churning every filter's signature.
/// Filters that don't need every part (e.g. the nonsensitive passthrough) simply ignore it.
/// </summary>
public sealed class FilterInput
{
    public string FieldName { get; }

    public string Value { get; }

    public IPseudonymContext Pseudonyms { get; }

    public FilterInput(string fieldName, string value, IPseudonymContext pseudonyms)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(pseudonyms);
        this.FieldName = fieldName;
        this.Value = value;
        this.Pseudonyms = pseudonyms;
    }
}
