using System.Security.Cryptography;

namespace Logger.Services;

/// <summary>
/// The production <see cref="ITokenGenerator"/> (REQ-0023): 128 bits of cryptographic randomness
/// (above the 120-bit minimum from DSS §6.1), rendered as hex.
/// </summary>
public sealed class SecureTokenGenerator : ITokenGenerator
{
    private const int TokenBytes = 16; // 128 bits

    public string NewToken() => Convert.ToHexString(RandomNumberGenerator.GetBytes(TokenBytes));
}
