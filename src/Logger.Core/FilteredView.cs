namespace Logger.Core;

/// <summary>
/// The filtered representation of one event (REQ-0014): each field's value after its disposition's
/// filter has been applied, in the schema's declared field order. This is what routine (filtered)
/// access sees — sensitive fields already wrapped as pseudonyms.
/// </summary>
public sealed class FilteredView
{
    private readonly IReadOnlyList<KeyValuePair<string, string>> fields;

    public FilteredView(IReadOnlyList<KeyValuePair<string, string>> fields)
    {
        ArgumentNullException.ThrowIfNull(fields);
        this.fields = fields;
    }

    /// <summary>The filtered fields, in the schema's declared order.</summary>
    public IReadOnlyList<KeyValuePair<string, string>> Fields => this.fields;

    public bool TryGetValue(string fieldName, out string? value)
    {
        ArgumentNullException.ThrowIfNull(fieldName);

        foreach (KeyValuePair<string, string> field in this.fields)
        {
            if (string.Equals(field.Key, fieldName, StringComparison.Ordinal))
            {
                value = field.Value;
                return true;
            }
        }

        value = null;
        return false;
    }
}
