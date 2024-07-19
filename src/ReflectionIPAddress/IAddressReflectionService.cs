using System;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Represents a service that retrieves the public IP address of the client.
    /// </summary>
    public interface IAddressReflectionService
    {
        /// <summary>
        /// Gets the URI of the service.
        /// </summary>
        Uri ServiceUri { get; }

        /// <summary>
        /// Gets the communication method used by the service.
        /// </summary>
        CommunicationMethods CommunicationMethod { get; }
    }
}
