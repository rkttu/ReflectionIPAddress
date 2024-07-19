using System;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Google's STUN service. (#3)
    /// </summary>
    public sealed class GoogleStun3Service : IStunAddressReflectionService
    {
        /// <summary>
        /// Gets the URI of the service.
        /// </summary>
        public Uri ServiceUri => new Uri("stun://stun3.l.google.com:19302", UriKind.Absolute);

        /// <summary>
        /// Gets the communication method used by the service.
        /// </summary>
        public CommunicationMethods CommunicationMethod => CommunicationMethods.UdpStun;
    }
}
