using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Extension methods for IP reflection services.
    /// </summary>
    public static class IPReflectionExtensions
    {
        /// <summary>
        /// Adds a service to the collection of services.
        /// </summary>
        /// <typeparam name="TService">
        /// The type of the service to add.
        /// </typeparam>
        /// <param name="services">
        /// The collection of services to add the service to.
        /// </param>
        /// <returns>
        /// The collection of services with the added service.
        /// </returns>
        public static PublicAddressReflectionServices AddService<TService>(this PublicAddressReflectionServices services)
            where TService : IPublicAddressReflectionService, new()
        {
            services.Add(new TService());
            return services;
        }

        /// <summary>
        /// Reflects the public IPv4 address using the specified services.
        /// </summary>
        /// <param name="services">
        /// The services to use for reflection.
        /// </param>
        /// <param name="bufferSize">
        /// The size of the buffer to use for reading the response stream.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The public IPv4 address of this computer.
        /// </returns>
        public static Task<IPAddress> ReflectIPv4Async(
            this IEnumerable<IPublicAddressReflectionService> services,
            int bufferSize = Constants.DefaultBufferSize, CancellationToken cancellationToken = default)
            => services.ReflectAsync(AddressFamily.InterNetwork, bufferSize, cancellationToken);

        /// <summary>
        /// Reflects the public IPv6 address using the specified services.
        /// </summary>
        /// <param name="services">
        /// The services to use for reflection.
        /// </param>
        /// <param name="bufferSize">
        /// The size of the buffer to use for reading the response stream.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The public IPv6 address of this computer.
        /// </returns>
        public static Task<IPAddress> ReflectIPv6Async(
            this IEnumerable<IPublicAddressReflectionService> services,
            int bufferSize = Constants.DefaultBufferSize, CancellationToken cancellationToken = default)
            => services.ReflectAsync(AddressFamily.InterNetworkV6, bufferSize, cancellationToken);

        /// <summary>
        /// Reflects the public IP address using the specified services.
        /// </summary>
        /// <param name="services">
        /// The services to use for reflection.
        /// </param>
        /// <param name="addressFamily">
        /// The address family to reflect.
        /// </param>
        /// <param name="bufferSize">
        /// The size of the buffer to use for reading the response stream.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The public IP address of this computer.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ReflectionIPAddressException"></exception>
        public static async Task<IPAddress> ReflectAsync(
            this IEnumerable<IPublicAddressReflectionService> services,
            AddressFamily addressFamily, int bufferSize = Constants.DefaultBufferSize,
            CancellationToken cancellationToken = default)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (services.Count() < 1)
                throw new ArgumentException("Please specify one or more services to use.", nameof(services));
            if (addressFamily != AddressFamily.InterNetwork &&
                addressFamily != AddressFamily.InterNetworkV6)
                throw new ArgumentException($"Selected address family is not supported - {addressFamily}", nameof(addressFamily));
            var taskList = services.Select(x => x.ReflectAsync(addressFamily, bufferSize, cancellationToken)).ToList();

            while (taskList.Any())
            {
                var completedTask = await Task.WhenAny(taskList).ConfigureAwait(false);
                taskList.Remove(completedTask);

                try { return await completedTask.ConfigureAwait(false); }
                catch { }
            }

            throw new ReflectionIPAddressException("Cannot obtain IP address from external services.");
        }

        /// <summary>
        /// Reflects the public IPv4 address using the specified service.
        /// </summary>
        /// <param name="service">
        /// The service to use for reflection.
        /// </param>
        /// <param name="bufferSize">
        /// The size of the buffer to use for reading the response stream.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The public IPv4 address of this computer.
        /// </returns>
        public static Task<IPAddress> ReflectIPv4Async(
            this IPublicAddressReflectionService service,
            int bufferSize = Constants.DefaultBufferSize, CancellationToken cancellationToken = default)
            => service.ReflectAsync(AddressFamily.InterNetwork, bufferSize, cancellationToken);

        /// <summary>
        /// Reflects the public IPv6 address using the specified service.
        /// </summary>
        /// <param name="service">
        /// The service to use for reflection.
        /// </param>
        /// <param name="bufferSize">
        /// The size of the buffer to use for reading the response stream.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The public IPv6 address of this computer.
        /// </returns>
        public static Task<IPAddress> ReflectIPv6Async(
            this IPublicAddressReflectionService service,
            int bufferSize = Constants.DefaultBufferSize, CancellationToken cancellationToken = default)
            => service.ReflectAsync(AddressFamily.InterNetworkV6, bufferSize, cancellationToken);

        /// <summary>
        /// Reflects the public IP address using the specified service.
        /// </summary>
        /// <param name="service">
        /// The service to use for reflection.
        /// </param>
        /// <param name="addressFamily">
        /// The address family to reflect.
        /// </param>
        /// <param name="bufferSize">
        /// The size of the buffer to use for reading the response stream.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The public IP address of this computer.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the selected address family is not supported.
        /// </exception>
        public static async Task<IPAddress> ReflectAsync(
            this IPublicAddressReflectionService service,
            AddressFamily addressFamily, int bufferSize = Constants.DefaultBufferSize,
            CancellationToken cancellationToken = default)
        {
            if (addressFamily != AddressFamily.InterNetwork &&
                addressFamily != AddressFamily.InterNetworkV6)
                throw new ArgumentException($"Selected address family is not supported - {addressFamily}", nameof(addressFamily));

            using (var responseStream = await service.CommunicateWithSpecificAddressFamilyAsync(addressFamily, bufferSize, cancellationToken).ConfigureAwait(false))
            {
                if (object.ReferenceEquals(Stream.Null, responseStream))
                    return null;

                var addr = await service.ParseResponse(responseStream, cancellationToken).ConfigureAwait(false);

                if (addr == default)
                    return null;

                return addr;
            }
        }

        /// <summary>
        /// Communicates with the service using the specified address family.
        /// </summary>
        /// <param name="service">
        /// The service to communicate with.
        /// </param>
        /// <param name="addressFamily">
        /// The address family to use for communication.
        /// </param>
        /// <param name="bufferSize">
        /// The size of the buffer to use for reading the response stream.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The stream containing the response from the service.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the service is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the URI scheme is not supported, or the URI has no host.
        /// </exception>
        /// <exception cref="ReflectionIPAddressException">
        /// Thrown when no address is found for the specified address family.
        /// </exception>
        private static async Task<Stream> CommunicateWithSpecificAddressFamilyAsync(
            this IPublicAddressReflectionService service, AddressFamily addressFamily, int bufferSize, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (bufferSize < Constants.DefaultBufferSize)
                bufferSize = Constants.DefaultBufferSize;
            var targetUri = service.ServiceUri;
            if (targetUri == null)
                throw new ArgumentNullException(nameof(IPublicAddressReflectionService.ServiceUri));
            if (!string.Equals(Uri.UriSchemeHttps, targetUri.Scheme, StringComparison.Ordinal) &&
                !string.Equals(Uri.UriSchemeHttp, targetUri.Scheme, StringComparison.Ordinal))
                throw new ArgumentException($"Selected URI has incompatible scheme - {targetUri.Scheme}", nameof(targetUri));

            var host = targetUri.Host;
            var port = targetUri.Port;
            var pathAndQuery = targetUri.PathAndQuery;

            var socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
            var ipHost = await Dns.GetHostEntryAsync(targetUri.Host).ConfigureAwait(false);
            var ipAddr = default(IPAddress);

            foreach (var address in ipHost.AddressList)
            {
                if (address.AddressFamily == addressFamily)
                {
                    ipAddr = address;
                    break;
                }
            }

            if (ipAddr == null)
                throw new ReflectionIPAddressException($"No {addressFamily} address found for {host}");

            IPEndPoint endPoint = new IPEndPoint(ipAddr, port);
            await Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect, endPoint, null).ConfigureAwait(false);

            NetworkStream networkStream = new NetworkStream(socket);
            SslStream sslStream = new SslStream(networkStream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            await Task.Factory.FromAsync<string>(sslStream.BeginAuthenticateAsClient, sslStream.EndAuthenticateAsClient, host, null).ConfigureAwait(false);

            using (var writer = new StreamWriter(sslStream, Encoding.ASCII, bufferSize, true) { AutoFlush = true })
            {
                string request = $"GET {pathAndQuery} HTTP/1.1\r\n" +
                                 $"Host: {host}\r\n" +
                                 $"User-Agent: IPReflection/1.0\r\n" +
                                 $"Accept: application/json\r\n" +
                                 $"Connection: close\r\n" +
                                 $"\r\n";
                await writer.WriteAsync(request).ConfigureAwait(false);
            }

            var buffer = new byte[4];
            var index = 0;

            while (true)
            {
                int b = sslStream.ReadByte();
                if (b < 0)
                    break;

                buffer[index] = (byte)b;
                index = (index + 1) % 4;

                // \r\n\r\n
                if (buffer[index] == 13 &&
                    buffer[(index + 1) % 4] == 10 &&
                    buffer[(index + 2) % 4] == 13 &&
                    buffer[(index + 3) % 4] == 10)
                    return sslStream;
            }

            return Stream.Null;
        }

        private static bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
            => sslPolicyErrors == SslPolicyErrors.None;

        /// <summary>
        /// Converts the IP address to a domain name using the "sslip.io" provider.
        /// </summary>
        /// <param name="address">
        /// The IP address to convert.
        /// </param>
        /// <param name="subDomain">
        /// The subdomain to use.
        /// </param>
        /// <param name="domain">
        /// The domain to use.
        /// </param>
        /// <param name="useDashSeparator">
        /// A value indicating whether to use a dash separator.
        /// </param>
        /// <returns>
        /// The wildcard domain name points to the IP address.
        /// </returns>
        public static string ToSSLIPDomain(this IPAddress address, string subDomain = null, string domain = null, bool useDashSeparator = true)
            => ToWildcardDomain(address, "sslip.io", subDomain, domain, useDashSeparator);

        /// <summary>
        /// Converts the IP address to a domain name using the "xip.io" type provider.
        /// </summary>
        /// <param name="address">
        /// The IP address to convert.
        /// </param>
        /// <param name="wildcardDomainProvider">
        /// The wildcard domain provider to use.
        /// </param>
        /// <param name="subDomain">
        /// The subdomain to use.
        /// </param>
        /// <param name="domain">
        /// The domain to use.
        /// </param>
        /// <param name="useDashSeparator">
        /// A value indicating whether to use a dash separator.
        /// </param>
        /// <returns>
        /// The wildcard domain name points to the IP address.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the wildcard domain provider is not specified.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the IP address is null.
        /// </exception>
        public static string ToWildcardDomain(this IPAddress address, string wildcardDomainProvider, string subDomain = null, string domain = null, bool useDashSeparator = true)
        {
            if (string.IsNullOrWhiteSpace(wildcardDomainProvider))
                throw new ArgumentException("Please specify wildcard domain provider.", nameof(wildcardDomainProvider));
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            string prefix = string.Empty;
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                prefix = address.ToString();
                if (!string.IsNullOrWhiteSpace(subDomain))
                    prefix = string.Join(".", subDomain, prefix);
                if (useDashSeparator)
                    prefix = prefix.Replace('.', '-');
            }
            else if (address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                prefix = address.ToString();
                prefix = prefix.Replace(':', '-');
                if (!string.IsNullOrWhiteSpace(subDomain))
                    prefix = string.Join("-", subDomain, prefix);
            }
            else
                throw new ArgumentException($"Unsupported address type '{address?.AddressFamily}' found.", nameof(address));

            var postfix = wildcardDomainProvider;
            if (!string.IsNullOrWhiteSpace(domain))
                postfix = domain;
            return string.Join(".", prefix, postfix);
        }
    }
}
