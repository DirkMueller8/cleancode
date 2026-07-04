using System.Net;
using System.Net.Sockets;

namespace Logger.Core;

/// <summary>
/// The filter for the <c>country</c> disposition (REQ-0012): replaces an IP address with a pseudonym
/// combining its country (from an injected <see cref="IGeoLookup"/>), a per-context sequence number,
/// and an address-family hint — e.g. <c>US1(v4)</c>. Geography stays useful while the address is
/// withheld. Uses the shared <see cref="Pseudonym"/> format, like <see cref="PrivateFilter"/>.
/// </summary>
public sealed class CountryFilter : IFieldFilter
{
    private readonly IGeoLookup geoLookup;

    public CountryFilter(IGeoLookup geoLookup)
    {
        ArgumentNullException.ThrowIfNull(geoLookup);
        this.geoLookup = geoLookup;
    }

    public string Apply(FilterInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        string country = this.geoLookup.CountryOf(input.Value);
        int sequence = input.Pseudonyms.SequenceFor(country, input.Value);
        return Pseudonym.Format(country, sequence, AddressFamilyHint(input.Value));
    }

    private static string AddressFamilyHint(string ipAddress)
    {
        IPAddress address = IPAddress.Parse(ipAddress);
        return address.AddressFamily == AddressFamily.InterNetworkV6 ? "v6" : "v4";
    }
}
