namespace Logger.Core;

/// <summary>
/// Builds the <see cref="FilteredView"/> of an event (REQ-0014) by applying, to each field, the filter
/// registered for that field's disposition. Pure composition over the filters (REQ-0007–0013): it
/// depends only on the <see cref="IFilterRegistry"/> and contains <b>no per-disposition logic</b> — it
/// never switches on type, so a filter it has never heard of works automatically. One
/// <see cref="IPseudonymContext"/> is shared across all fields so pseudonyms are consistent within the
/// event. Precondition: the event has been validated against the schema (REQ-0005).
/// </summary>
public sealed class FilteredViewAssembler
{
    private readonly IFilterRegistry filterRegistry;

    public FilteredViewAssembler(IFilterRegistry filterRegistry)
    {
        ArgumentNullException.ThrowIfNull(filterRegistry);
        this.filterRegistry = filterRegistry;
    }

    public FilteredView Assemble(LogType logType, LogEvent logEvent, IPseudonymContext context)
    {
        ArgumentNullException.ThrowIfNull(logType);
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(context);

        var filtered = new List<KeyValuePair<string, string>>();
        foreach (FieldDefinition field in logType.Fields)
        {
            if (!logEvent.Values.TryGetValue(field.Name, out string? rawValue))
            {
                throw new ArgumentException(
                    $"Event is missing declared field '{field.Name}' — validate the event before assembling.",
                    nameof(logEvent));
            }

            IFieldFilter filter = this.filterRegistry.Resolve(field.Disposition.Name);
            string filteredValue = filter.Apply(new FilterInput(field.Name, rawValue, context));
            filtered.Add(new KeyValuePair<string, string>(field.Name, filteredValue));
        }

        return new FilteredView(filtered);
    }
}
