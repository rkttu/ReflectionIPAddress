using System;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Represents a service that retrieves the public IP address of the client.
    /// </summary>
    public sealed class ReflectionIPAddressException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionIPAddressException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public ReflectionIPAddressException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionIPAddressException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception.
        /// </param>
        public ReflectionIPAddressException(string message, Exception innerException) : base(message, innerException) { }
    }
}
