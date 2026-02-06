using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Represents a service that can retrieve detailed IP address information including geographic and network metadata.
    /// </summary>
    public interface IRichAddressReflectionService : IPublicAddressReflectionService
    {
        /// <summary>
        /// Parses the response from the service and returns detailed IP address information.
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
        Task<IPAddressInfo> ParseDetailedResponse(Stream stream, CancellationToken cancellationToken = default);
    }
}
