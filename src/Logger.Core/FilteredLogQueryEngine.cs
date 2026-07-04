namespace Logger.Core;

/// <summary>
/// Searches already-filtered log entries by their symbolic identifiers (REQ-0019) and refuses
/// inference attacks (REQ-0020). It is told which fields are pseudonymized (the service layer derives
/// this from the schema's dispositions); for those fields a query value must be a <i>symbol</i> the
/// user can already see (e.g. <c>US1</c>) — a raw/guessed value (e.g. <c>1.1.1.1</c>) is rejected, so a
/// hidden value can never be confirmed by trial. Matching ignores the format/length hint, so
/// <c>US1</c> matches <c>US1(v4)</c>. The engine never touches raw data.
/// </summary>
public sealed class FilteredLogQueryEngine
{
    private readonly HashSet<string> pseudonymFields;

    public FilteredLogQueryEngine(IEnumerable<string> pseudonymFields)
    {
        ArgumentNullException.ThrowIfNull(pseudonymFields);
        this.pseudonymFields = new HashSet<string>(pseudonymFields, StringComparer.Ordinal);
    }

    public IReadOnlyList<FilteredView> Search(
        IEnumerable<FilteredView> entries,
        IReadOnlyList<QueryCondition> conditions)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(conditions);

        this.RejectRawValueQueries(conditions);

        var matches = new List<FilteredView>();
        foreach (FilteredView entry in entries)
        {
            if (Matches(entry, conditions))
            {
                matches.Add(entry);
            }
        }

        return matches;
    }

    // REQ-0020: a pseudonymized field may only be queried by a symbol, never a raw/guessed value.
    private void RejectRawValueQueries(IReadOnlyList<QueryCondition> conditions)
    {
        foreach (QueryCondition condition in conditions)
        {
            if (this.pseudonymFields.Contains(condition.FieldName) && !IsSymbol(condition.Value))
            {
                throw new ArgumentException(
                    $"Field '{condition.FieldName}' is pseudonymized; query it by its symbol (e.g. US1), " +
                    $"not by an exact value ('{condition.Value}').",
                    nameof(conditions));
            }
        }
    }

    private static bool Matches(FilteredView entry, IReadOnlyList<QueryCondition> conditions)
    {
        foreach (QueryCondition condition in conditions)
        {
            if (!entry.TryGetValue(condition.FieldName, out string? filtered)
                || !string.Equals(Symbol(filtered!), condition.Value, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>The identifier part of a filtered value — everything before the format/length hint.</summary>
    private static string Symbol(string filteredValue)
    {
        int hint = filteredValue.IndexOf('(', StringComparison.Ordinal);
        return hint >= 0 ? filteredValue[..hint] : filteredValue;
    }

    // A pseudonym symbol is one or more uppercase letters followed by one or more digits (US1, USER1, PW1).
    private static bool IsSymbol(string value)
    {
        int i = 0;
        while (i < value.Length && char.IsAsciiLetterUpper(value[i]))
        {
            i++;
        }

        if (i == 0)
        {
            return false;
        }

        int firstDigit = i;
        while (i < value.Length && char.IsAsciiDigit(value[i]))
        {
            i++;
        }

        return i > firstDigit && i == value.Length;
    }
}
