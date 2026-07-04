using Logger.Core;

namespace Logger.Services;

/// <summary>
/// The logging session RPC as a state machine (REQ-0022/0023/0024): <c>Hello → Schema → Event* →
/// Goodbye</c> over states New → Active → Closed. Out-of-order verbs are a sequencing/contract violation
/// and throw <see cref="InvalidOperationException"/>; invalid event <i>data</i> is expected input and
/// comes back as a <see cref="ValidationResult"/> (REQ-0005) — the mechanism matches the kind of failure.
/// </summary>
public sealed class LoggingSession
{
    private readonly ITokenGenerator tokenGenerator;
    private readonly ILogStore store;

    private SessionState state = SessionState.New;
    private string? token;
    private SchemaValidator? validator;

    public LoggingSession(ITokenGenerator tokenGenerator, ILogStore store)
    {
        ArgumentNullException.ThrowIfNull(tokenGenerator);
        ArgumentNullException.ThrowIfNull(store);
        this.tokenGenerator = tokenGenerator;
        this.store = store;
    }

    private enum SessionState
    {
        New,
        Active,
        Closed,
    }

    public bool IsActive => this.state == SessionState.Active;

    public string? Token => this.token;

    /// <summary>Starts the session and issues a token (REQ-0023). Must be the first verb.</summary>
    public string Hello()
    {
        this.RequireState(SessionState.New, "Hello");
        this.token = this.tokenGenerator.NewToken();
        this.state = SessionState.Active;
        return this.token;
    }

    /// <summary>Defines the schema for subsequent events. Only while Active.</summary>
    public void DefineSchema(Schema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);
        this.RequireState(SessionState.Active, "Schema");
        this.validator = new SchemaValidator(schema);
    }

    /// <summary>Records an event if it conforms to the schema; returns why it was rejected if not.</summary>
    public ValidationResult RecordEvent(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        this.RequireState(SessionState.Active, "Event");
        if (this.validator is null)
        {
            throw new InvalidOperationException("A schema must be defined (Schema) before recording events.");
        }

        ValidationResult result = this.validator.Validate(logEvent);
        if (result.IsValid)
        {
            this.store.Append(logEvent);
        }

        return result;
    }

    /// <summary>Ends the session and invalidates the token (REQ-0024). The session is then terminal.</summary>
    public void Goodbye()
    {
        this.RequireState(SessionState.Active, "Goodbye");
        this.state = SessionState.Closed;
        this.token = null;
    }

    private void RequireState(SessionState required, string verb)
    {
        if (this.state != required)
        {
            throw new InvalidOperationException($"'{verb}' is not allowed while the session is {this.state}.");
        }
    }
}
