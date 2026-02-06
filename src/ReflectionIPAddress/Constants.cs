using System;
using System.Text;
using System.Threading;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Represents a service that retrieves the public IP address of the client.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The default buffer size for reading the response stream.
        /// </summary>
        public const int DefaultBufferSize = 1024;

        /// <summary>
        /// The default timeout for sending a request to the service.
        /// </summary>
        public const int UdpDefaultSendTimeoutMilliseconds = 5000;

        /// <summary>
        /// The default timeout for receiving a response from the service.
        /// </summary>
        public const int UdpDefaultReceiveTimeoutMilliseconds = 5000;

        /// <summary>
        /// The default timeout in milliseconds for the entire HTTP-based IP reflection operation.
        /// A value of 0 means no timeout.
        /// </summary>
        public const int HttpDefaultTimeoutMilliseconds = 15000;

        internal static readonly Lazy<Encoding> UTF8NoBOMEncoding = new Lazy<Encoding>(
            () => new UTF8Encoding(false), LazyThreadSafetyMode.None);

        internal static readonly Lazy<char[]> CommaSeparators = new Lazy<char[]>(
            () => new char[] { ',', }, LazyThreadSafetyMode.None);

        internal static readonly Lazy<Random> SharedRandom = new Lazy<Random>(
            () => new Random(), LazyThreadSafetyMode.None);
    }
}
