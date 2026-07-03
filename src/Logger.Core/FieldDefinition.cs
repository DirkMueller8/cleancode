namespace Logger.Core;

/// <summary>
/// One named, typed field within a <see cref="LogType"/> (REQ-0002), with its sensitivity
/// <see cref="Disposition"/> (REQ-0004). Immutable value object. Because the disposition is a required,
/// non-null constructor argument, a field cannot be constructed without declaring one — "every field
/// must declare a disposition" is enforced structurally.
/// </summary>
public sealed class FieldDefinition
{
    public string Name { get; }

    public FieldType Type { get; }

    public Disposition Disposition { get; }

    public FieldDefinition(string name, FieldType type, Disposition disposition)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(disposition);
        this.Name = name;
        this.Type = type;
        this.Disposition = disposition;
    }
}
