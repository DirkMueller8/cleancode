namespace Logger.Core;

/// <summary>
/// The Strategy abstraction for turning a raw field value into its filtered-view form (Epic B).
/// One implementation per disposition (nonsensitive, private, minute, country, custom…); the filtered
/// view assembler (REQ-0014) resolves the right one by disposition name via the <see cref="IFilterRegistry"/>.
/// <para>
/// The parameter grew from a bare value (REQ-0007) to a <see cref="FilterInput"/> at REQ-0008, when
/// private/pseudonym filters needed the field name and a pseudonym context — an interface earning its
/// parameters as requirements reveal the need, rather than being designed omnisciently up front.
/// </para>
/// </summary>
public interface IFieldFilter
{
    /// <summary>Returns the filtered-view representation of the input's value.</summary>
    string Apply(FilterInput input);
}
