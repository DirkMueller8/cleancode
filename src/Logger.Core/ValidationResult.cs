namespace Logger.Core;

/// <summary>
/// The outcome of validating a <see cref="LogEvent"/> against a schema (REQ-0005). A <i>result</i>,
/// not an exception: an invalid event is an expected runtime outcome (events stream in from
/// possibly-untrusted sources), so the caller must inspect <see cref="IsValid"/> rather than rely on a
/// throw (ISO/IEC 24772-1 [OYB] — don't ignore an error status). Collects <b>all</b> violations, not
/// just the first.
/// </summary>
public sealed class ValidationResult
{
    public IReadOnlyList<string> Violations { get; }

    public bool IsValid => this.Violations.Count == 0;

    private ValidationResult(IReadOnlyList<string> violations) => this.Violations = violations;

    public static ValidationResult Valid { get; } = new(Array.Empty<string>());

    public static ValidationResult Invalid(IReadOnlyList<string> violations)
    {
        ArgumentNullException.ThrowIfNull(violations);
        if (violations.Count == 0)
        {
            throw new ArgumentException("An invalid result must list at least one violation.", nameof(violations));
        }

        return new ValidationResult(violations);
    }
}
