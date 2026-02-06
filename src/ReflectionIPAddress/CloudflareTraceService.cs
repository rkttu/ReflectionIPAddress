using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Represents a service that retrieves the public IP address of the client using the "cloudflare.com" trace service.
    /// </summary>
    public sealed class CloudflareTraceService : IPublicAddressReflectionService
    {
        private static readonly Regex IpLineRegex = new Regex(
            @"(?<Name>ip)=(?<Value>.+)",
            RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the URI of the service.
        /// </summary>
        public Uri ServiceUri => new Uri("https://www.cloudflare.com/cdn-cgi/trace", UriKind.Absolute);

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
            using (var reader = new StreamReader(stream, Constants.UTF8NoBOMEncoding.Value, true))
            {
                var str = await reader.ReadToEndAsync().ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(str))
                    return default;

                var match = IpLineRegex.Match(str);

                if (!match.Success || !IPAddress.TryParse(match.Groups["Value"].Value.Trim(), out IPAddress addr))
                    return default;

                return addr;
            }
        }
    }
}
