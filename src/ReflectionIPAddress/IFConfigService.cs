using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Represents a service that retrieves the public IP address of the client using the "ifconfig.co" service.
    /// This service also supports retrieving detailed information including country, city, ASN, and time zone.
    /// </summary>
    public sealed class IFConfigService : IRichAddressReflectionService
    {
        /// <summary>
        /// Gets the URI of the service.
        /// </summary>
        public Uri ServiceUri => new Uri("https://ifconfig.co/json", UriKind.Absolute);

        /// <summary>
        /// Gets the communication method used by the service.
        /// </summary>
        public CommunicationMethods CommunicationMethod => CommunicationMethods.TcpHttps;

        /// <summary>
        /// Parses the response from the service and returns the IP address.
        /// </summary>
        /// <param name="stream">
        /// The stream containing the response from the service.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The IP address parsed from the response.
        /// </returns>
        public async Task<IPAddress> ParseResponse(Stream stream, CancellationToken cancellationToken = default)
        {
            var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!doc.RootElement.TryGetProperty("ip", out JsonElement ipElement))
                return default;

            if (!IPAddress.TryParse(ipElement.GetString(), out IPAddress ipAddress))
                return default;

            return ipAddress;
        }

        /// <summary>
        /// Parses the response from the service and returns detailed IP address information
        /// including country, city, ASN, time zone, and geographic coordinates.
        /// </summary>
        /// <param name="stream">
        /// The stream containing the response from the service.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The detailed IP address information parsed from the response.
        /// </returns>
        public async Task<IPAddressInfo> ParseDetailedResponse(Stream stream, CancellationToken cancellationToken = default)
        {
            var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
            var root = doc.RootElement;
            var info = new IPAddressInfo();

            if (root.TryGetProperty("ip", out JsonElement ipElement) &&
                IPAddress.TryParse(ipElement.GetString(), out IPAddress ipAddress))
                info.Address = ipAddress;

            if (root.TryGetProperty("country", out JsonElement countryElement))
                info.Country = countryElement.GetString();

            if (root.TryGetProperty("country_iso", out JsonElement countryIsoElement))
                info.CountryISO = countryIsoElement.GetString();

            if (root.TryGetProperty("city", out JsonElement cityElement))
                info.City = cityElement.GetString();

            if (root.TryGetProperty("hostname", out JsonElement hostnameElement))
                info.Hostname = hostnameElement.GetString();

            if (root.TryGetProperty("asn", out JsonElement asnElement))
                info.ASN = ParseIntFromString(asnElement.GetString());

            if (root.TryGetProperty("asn_org", out JsonElement asnOrgElement))
                info.ASNOrganization = asnOrgElement.GetString();

            if (root.TryGetProperty("time_zone", out JsonElement tzElement))
                info.TimeZone = tzElement.GetString();

            if (root.TryGetProperty("latitude", out JsonElement latElement) &&
                latElement.ValueKind == JsonValueKind.Number)
                info.Latitude = latElement.GetDouble();

            if (root.TryGetProperty("longitude", out JsonElement lonElement) &&
                lonElement.ValueKind == JsonValueKind.Number)
                info.Longitude = lonElement.GetDouble();

            return info;
        }

        private static int? ParseIntFromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // ASN may come as "ASXXXXX" format
            var numStr = value;
            if (numStr.StartsWith("AS", StringComparison.OrdinalIgnoreCase))
                numStr = numStr.Substring(2);

            if (int.TryParse(numStr, out int result))
                return result;

            return null;
        }
    }
}
