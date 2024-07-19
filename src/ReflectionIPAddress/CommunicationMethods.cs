namespace ReflectionIPAddress
{
    /// <summary>
    /// Represents the communication methods that can be used to retrieve the public IP address of the client.
    /// </summary>
    public enum CommunicationMethods : int
    {
        /// <summary>
        /// The communication method is undefined.
        /// </summary>
        Undefined,

        /// <summary>
        /// The communication method is HTTP.
        /// </summary>
        TcpHttp,

        /// <summary>
        /// The communication method is HTTPS.
        /// </summary>
        TcpHttps,

        /// <summary>
        /// The communication method is UDP.
        /// </summary>
        UdpStun,
    }
}
