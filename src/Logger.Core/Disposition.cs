namespace Logger.Core;

/// <summary>
/// A field's sensitivity-handling rule (REQ-0004): identified by the name of the filter that
/// produces the field's filtered view. Well-known names are exposed as convenience members, but this
/// type carries <b>no special-casing</b> — whether a disposition name is valid is decided by the
/// <see cref="IFilterRegistry"/> at schema-build time (consistent with REQ-0013).
/// Names are compared case-sensitively, like all names in the model (see glossary → <i>name</i>).
/// </summary>
public sealed class Disposition
{
    public const string NonsensitiveName = "nonsensitive";
    public const string PrivateName = "private";

    /// <summary>Data that carries no privacy risk and is copied unchanged into the filtered view (REQ-0007).</summary>
    public static readonly Disposition Nonsensitive = new(NonsensitiveName);

    /// <summary>Data that must be wrapped as a pseudonym in the filtered view (REQ-0008).</summary>
    public static readonly Disposition Private = new(PrivateName);

    public string Name { get; }

    public Disposition(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        this.Name = name;
    }
}
