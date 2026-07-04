namespace Logger.Core;

/// <summary>
/// Enforces the input-size limits in <see cref="Limits"/> (REQ-0006), at the input boundary and
/// <b>before</b> schema validation. Returns a <see cref="ValidationResult"/> (does not throw): an
/// oversized input is an expected, possibly hostile request, not a programming error. The two checks
/// are separate because they apply to different inputs — the whole raw request vs. one parsed event.
/// </summary>
public sealed class SizeValidator
{
    /// <summary>Rejects a raw request whose text exceeds <see cref="Limits.MaxRequestChars"/>.</summary>
    public ValidationResult ValidateRequestSize(string request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Length > Limits.MaxRequestChars)
        {
            return ValidationResult.Invalid(new[]
            {
                $"Request length {request.Length} exceeds the maximum of {Limits.MaxRequestChars} characters.",
            });
        }

        return ValidationResult.Valid;
    }

    /// <summary>Rejects any field value exceeding <see cref="Limits.MaxFieldValueChars"/>, naming the field.</summary>
    public ValidationResult ValidateFieldSizes(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        var violations = new List<string>();
        foreach ((string name, string value) in logEvent.Values)
        {
            if (value.Length > Limits.MaxFieldValueChars)
            {
                violations.Add(
                    $"Field '{name}' value length {value.Length} exceeds the maximum of " +
                    $"{Limits.MaxFieldValueChars} characters.");
            }
        }

        return violations.Count == 0 ? ValidationResult.Valid : ValidationResult.Invalid(violations);
    }
}
