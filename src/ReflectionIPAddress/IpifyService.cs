using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Represents a service that retrieves the public IP address of the client using the "ipify.org" service.
    /// </summary>
    public sealed class IpifyService : IPublicAddressReflectionService
    {
        /// <summary>
        /// Gets the URI of the service.
        /// </summary>
        public Uri ServiceUri => new Uri("https://api64.ipify.org/?format=json", UriKind.Absolute);

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
    }
}
