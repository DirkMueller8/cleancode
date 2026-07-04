using System.Net;

namespace Logger.Core;

/// <summary>
/// Validates a <see cref="LogEvent"/> against a <see cref="Schema"/> (REQ-0005): an event is accepted
/// only if its fields match the declared names and types of its LogType. Rejects missing fields,
/// undeclared ("unknown") fields, values that don't parse to the declared type, and events for an
/// unknown LogType. Kept separate from filtering (SRP). Returns a <see cref="ValidationResult"/>
/// (does not throw) because an invalid event is an expected outcome, not a programming error.
/// </summary>
public sealed class SchemaValidator
{
    private readonly Schema schema;

    public SchemaValidator(Schema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);
        this.schema = schema;
    }

    public ValidationResult Validate(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        if (!this.schema.TryGetLogType(logEvent.LogTypeName, out LogType? logType))
        {
            return ValidationResult.Invalid(new[] { $"Unknown log type '{logEvent.LogTypeName}'." });
        }

        var violations = new List<string>();

        foreach (FieldDefinition field in logType!.Fields)
        {
            if (!logEvent.Values.TryGetValue(field.Name, out string? value))
            {
                violations.Add($"Missing field '{field.Name}'.");
            }
            else if (!IsValueOfType(value, field.Type))
            {
                violations.Add($"Field '{field.Name}' value '{value}' is not a valid {field.Type}.");
            }
        }

        foreach (string providedName in logEvent.Values.Keys)
        {
            if (!logType.TryGetField(providedName, out _))
            {
                violations.Add($"Unknown field '{providedName}' not declared in log type '{logType.Name}'.");
            }
        }

        return violations.Count == 0 ? ValidationResult.Valid : ValidationResult.Invalid(violations);
    }

    // Checks a text value against the declared field type (ISO 24772 [FLC] — conversion errors).
    // An out-of-range FieldType falls through to false ([CCB] — enum values are not trusted blindly).
    private static bool IsValueOfType(string value, FieldType type) => type switch
    {
        FieldType.String => true,
        FieldType.Integer => long.TryParse(value, out _),
        FieldType.Time => long.TryParse(value, out _),          // Unix epoch seconds — the canonical form
        FieldType.IpAddress => IPAddress.TryParse(value, out _),
        _ => false,
    };
}
