using System;
using System.Text;

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

        internal static readonly Lazy<Encoding> UTF8NoBOMEncoding = new Lazy<Encoding>(
            () => new UTF8Encoding(false), false);

        internal static readonly Lazy<char[]> CommaSeparators = new Lazy<char[]>(
            () => new char[] { ',', }, false);
    }
}
