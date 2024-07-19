using System;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Query any of several Google STUN servers.
    /// </summary>
    public sealed class DistributedGoogleStunService : IStunAddressReflectionService
    {
        private Uri[] _googleStunServiceUris = new Uri[]
        {
            new Uri("stun://stun.l.google.com:19302", UriKind.Absolute),
            new Uri("stun://stun1.l.google.com:19302", UriKind.Absolute),
            new Uri("stun://stun2.l.google.com:19302", UriKind.Absolute),
            new Uri("stun://stun3.l.google.com:19302", UriKind.Absolute),
            new Uri("stun://stun4.l.google.com:19302", UriKind.Absolute),
        };

        /// <summary>
        /// Gets the URI of the service.
        /// </summary>
        public Uri ServiceUri => _googleStunServiceUris[Constants.SharedRandom.Value.Next(0, _googleStunServiceUris.Length - 1)];

        /// <summary>
        /// Gets the communication method used by the service.
        /// </summary>
        public CommunicationMethods CommunicationMethod => CommunicationMethods.UdpStun;
    }
}
