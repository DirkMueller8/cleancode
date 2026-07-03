namespace Logger.Core;

/// <summary>
/// One named, typed field within a <see cref="LogType"/> (REQ-0002).
/// Immutable value object. A field's sensitivity <c>Disposition</c> is added by REQ-0004;
/// this type deliberately covers structure only.
/// </summary>
public sealed class FieldDefinition
{
    public string Name { get; }

    public FieldType Type { get; }

    public FieldDefinition(string name, FieldType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        this.Name = name;
        this.Type = type;
    }
}
