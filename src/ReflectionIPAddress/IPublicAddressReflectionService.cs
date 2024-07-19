using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Represents a service that retrieves the public IP address of the client using HTTP/HTTPS Protocol.
    /// </summary>
    public interface IPublicAddressReflectionService : IAddressReflectionService
    {
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
