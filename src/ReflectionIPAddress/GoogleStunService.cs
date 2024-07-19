using System;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Google's STUN service.
    /// </summary>
    public sealed class GoogleStunService : IStunAddressReflectionService
    {
        /// <summary>
        /// Gets the URI of the service.
        /// </summary>
        public Uri ServiceUri => new Uri("stun://stun.l.google.com:19302", UriKind.Absolute);

        /// <summary>
        /// Gets the communication method used by the service.
        /// </summary>
        public CommunicationMethods CommunicationMethod => CommunicationMethods.UdpStun;
    }
}
