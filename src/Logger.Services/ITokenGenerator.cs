namespace Logger.Services;

/// <summary>
/// Produces session tokens (REQ-0023). Injected (DIP) so lifecycle tests can substitute a deterministic
/// token, while production uses a cryptographically strong generator.
/// </summary>
public interface ITokenGenerator
{
    string NewToken();
}
