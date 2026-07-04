namespace Logger.Core;

/// <summary>
/// The Strategy abstraction for turning a raw field value into its filtered-view form (Epic B).
/// One implementation per disposition (nonsensitive, private, minute, country, custom…); the filtered
/// view assembler (REQ-0014) resolves the right one by disposition name via the <see cref="IFilterRegistry"/>.
/// <para>
/// The interface is deliberately minimal for now (REQ-0007): a nonsensitive passthrough needs only the
/// value. It will <b>grow</b> when REQ-0008 introduces a pseudonym context that private/country filters
/// require — interfaces earn their parameters as requirements reveal the need, rather than being
/// designed omnisciently up front.
/// </para>
/// </summary>
public interface IFieldFilter
{
    /// <summary>Returns the filtered-view representation of <paramref name="value"/>.</summary>
    string Apply(string value);
}
