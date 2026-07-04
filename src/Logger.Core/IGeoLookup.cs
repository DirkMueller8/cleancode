namespace Logger.Core;

/// <summary>
/// Maps an IP address to its country of origin (REQ-0012). An injected seam (DIP): a real lookup needs
/// a geo database, which is out of scope for the core, so the composition root supplies an
/// implementation and tests stub it.
/// </summary>
public interface IGeoLookup
{
    /// <summary>Returns the country code (e.g. "US") for the given IP address.</summary>
    string CountryOf(string ipAddress);
}
