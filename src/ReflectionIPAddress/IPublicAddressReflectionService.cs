using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Represents a service that retrieves the public IP address of the client.
    /// </summary>
    public interface IPublicAddressReflectionService
    {
        /// <summary>
        /// Gets the URI of the service.
        /// </summary>
        Uri ServiceUri { get; }

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
        Task<IPAddress> ParseResponse(Stream stream, CancellationToken cancellationToken = default);
    }
}
