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
            where TService : IAddressReflectionService, new()
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
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The public IPv4 address of this computer.
        /// </returns>
        public static Task<IPAddress> ReflectIPv4Async(
            this IEnumerable<IAddressReflectionService> services,
            CancellationToken cancellationToken = default)
            => services.ReflectAsync(AddressFamily.InterNetwork, cancellationToken);

        /// <summary>
        /// Reflects the public IPv4 address using the specified services with a per-service timeout.
        /// </summary>
        /// <param name="services">
        /// The services to use for reflection.
        /// </param>
        /// <param name="perServiceTimeout">
        /// The maximum time allowed for each individual service request.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The public IPv4 address of this computer.
        /// </returns>
        public static Task<IPAddress> ReflectIPv4Async(
            this IEnumerable<IAddressReflectionService> services,
            TimeSpan perServiceTimeout,
            CancellationToken cancellationToken = default)
            => services.ReflectAsync(AddressFamily.InterNetwork, perServiceTimeout, cancellationToken);

        /// <summary>
        /// Reflects the public IPv6 address using the specified services.
        /// </summary>
        /// <param name="services">
        /// The services to use for reflection.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The public IPv6 address of this computer.
        /// </returns>
        public static Task<IPAddress> ReflectIPv6Async(
            this IEnumerable<IAddressReflectionService> services,
            CancellationToken cancellationToken = default)
            => services.ReflectAsync(AddressFamily.InterNetworkV6, cancellationToken);

        /// <summary>
        /// Reflects the public IPv6 address using the specified services with a per-service timeout.
        /// </summary>
        /// <param name="services">
        /// The services to use for reflection.
        /// </param>
        /// <param name="perServiceTimeout">
        /// The maximum time allowed for each individual service request.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The public IPv6 address of this computer.
        /// </returns>
        public static Task<IPAddress> ReflectIPv6Async(
            this IEnumerable<IAddressReflectionService> services,
            TimeSpan perServiceTimeout,
            CancellationToken cancellationToken = default)
            => services.ReflectAsync(AddressFamily.InterNetworkV6, perServiceTimeout, cancellationToken);

        /// <summary>
        /// Reflects the public IP address using the specified services.
        /// </summary>
        /// <param name="services">
        /// The services to use for reflection.
        /// </param>
        /// <param name="addressFamily">
        /// The address family to reflect.
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
        public static Task<IPAddress> ReflectAsync(
            this IEnumerable<IAddressReflectionService> services,
            AddressFamily addressFamily,
            CancellationToken cancellationToken = default)
            => services.ReflectAsync(addressFamily, TimeSpan.Zero, cancellationToken);

        /// <summary>
        /// Reflects the public IP address using the specified services with a per-service timeout.
        /// </summary>
        /// <param name="services">
        /// The services to use for reflection.
        /// </param>
        /// <param name="addressFamily">
        /// The address family to reflect.
        /// </param>
        /// <param name="perServiceTimeout">
        /// The maximum time allowed for each individual service request. Use <see cref="TimeSpan.Zero"/> for no timeout.
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
            this IEnumerable<IAddressReflectionService> services,
            AddressFamily addressFamily,
            TimeSpan perServiceTimeout,
            CancellationToken cancellationToken = default)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (services.Count() < 1)
                throw new ArgumentException("Please specify one or more services to use.", nameof(services));
            if (addressFamily != AddressFamily.InterNetwork &&
                addressFamily != AddressFamily.InterNetworkV6)
                throw new ArgumentException($"Selected address family is not supported - {addressFamily}", nameof(addressFamily));

            var timeoutMs = (int)perServiceTimeout.TotalMilliseconds;
            var taskList = services.Select(x => x.ReflectAsync(addressFamily, timeoutMs, cancellationToken)).ToList();

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
        /// Reflects the public IPv4 address from all specified services, returning results from every service that responds successfully.
        /// </summary>
        /// <param name="services">
        /// The services to use for reflection.
        /// </param>
        /// <param name="perServiceTimeout">
        /// The maximum time allowed for each individual service request. Use <see cref="TimeSpan.Zero"/> for no timeout.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A read-only dictionary mapping each service type to its returned IP address.
        /// </returns>
        public static Task<IReadOnlyDictionary<Type, IPAddress>> ReflectAllIPv4Async(
            this IEnumerable<IAddressReflectionService> services,
            TimeSpan perServiceTimeout = default,
            CancellationToken cancellationToken = default)
            => services.ReflectAllAsync(AddressFamily.InterNetwork, perServiceTimeout, cancellationToken);

        /// <summary>
        /// Reflects the public IPv6 address from all specified services, returning results from every service that responds successfully.
        /// </summary>
        /// <param name="services">
        /// The services to use for reflection.
        /// </param>
        /// <param name="perServiceTimeout">
        /// The maximum time allowed for each individual service request. Use <see cref="TimeSpan.Zero"/> for no timeout.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A read-only dictionary mapping each service type to its returned IP address.
        /// </returns>
        public static Task<IReadOnlyDictionary<Type, IPAddress>> ReflectAllIPv6Async(
            this IEnumerable<IAddressReflectionService> services,
            TimeSpan perServiceTimeout = default,
            CancellationToken cancellationToken = default)
            => services.ReflectAllAsync(AddressFamily.InterNetworkV6, perServiceTimeout, cancellationToken);

        /// <summary>
        /// Reflects the public IP address from all specified services, returning results from every service that responds successfully.
        /// This is useful for consensus-based validation where you want to compare results across multiple services.
        /// </summary>
        /// <param name="services">
        /// The services to use for reflection.
        /// </param>
        /// <param name="addressFamily">
        /// The address family to reflect.
        /// </param>
        /// <param name="perServiceTimeout">
        /// The maximum time allowed for each individual service request. Use <see cref="TimeSpan.Zero"/> for no timeout.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A read-only dictionary mapping each service type to its returned IP address.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<IReadOnlyDictionary<Type, IPAddress>> ReflectAllAsync(
            this IEnumerable<IAddressReflectionService> services,
            AddressFamily addressFamily,
            TimeSpan perServiceTimeout = default,
            CancellationToken cancellationToken = default)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (services.Count() < 1)
                throw new ArgumentException("Please specify one or more services to use.", nameof(services));
            if (addressFamily != AddressFamily.InterNetwork &&
                addressFamily != AddressFamily.InterNetworkV6)
                throw new ArgumentException($"Selected address family is not supported - {addressFamily}", nameof(addressFamily));

            var timeoutMs = (int)perServiceTimeout.TotalMilliseconds;
            var results = new Dictionary<Type, IPAddress>();
            var taskMap = services.ToDictionary(
                s => s.ReflectAsync(addressFamily, timeoutMs, cancellationToken),
                s => s.GetType());

            var taskList = taskMap.Keys.ToList();

            while (taskList.Any())
            {
                var completedTask = await Task.WhenAny(taskList).ConfigureAwait(false);
                taskList.Remove(completedTask);

                try
                {
                    var address = await completedTask.ConfigureAwait(false);
                    if (address != null && taskMap.TryGetValue(completedTask, out var serviceType))
                        results[serviceType] = address;
                }
                catch { }
            }

            return results;
        }

        /// <summary>
        /// Returns the most common (consensus) IP address from the results of all services.
        /// </summary>
        /// <param name="results">
        /// The results from <see cref="ReflectAllAsync"/>.
        /// </param>
        /// <returns>
        /// The IP address that appeared most frequently, or null if no results were obtained.
        /// </returns>
        public static IPAddress GetConsensusAddress(this IReadOnlyDictionary<Type, IPAddress> results)
        {
            if (results == null || results.Count == 0)
                return null;

            return results.Values
                .GroupBy(addr => addr.ToString())
                .OrderByDescending(g => g.Count())
                .First()
                .First();
        }

        /// <summary>
        /// Reflects the public IPv4 address using the specified service.
        /// </summary>
        /// <param name="service">
        /// The service to use for reflection.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The public IPv4 address of this computer.
        /// </returns>
        public static Task<IPAddress> ReflectIPv4Async(
            this IAddressReflectionService service,
            CancellationToken cancellationToken = default)
            => service.ReflectAsync(AddressFamily.InterNetwork, cancellationToken);

        /// <summary>
        /// Reflects the public IPv4 address using the specified service with a timeout.
        /// </summary>
        /// <param name="service">
        /// The service to use for reflection.
        /// </param>
        /// <param name="timeout">
        /// The maximum time allowed for the request.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The public IPv4 address of this computer.
        /// </returns>
        public static Task<IPAddress> ReflectIPv4Async(
            this IAddressReflectionService service,
            TimeSpan timeout,
            CancellationToken cancellationToken = default)
            => service.ReflectAsync(AddressFamily.InterNetwork, (int)timeout.TotalMilliseconds, cancellationToken);

        /// <summary>
        /// Reflects the public IPv6 address using the specified service.
        /// </summary>
        /// <param name="service">
        /// The service to use for reflection.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The public IPv6 address of this computer.
        /// </returns>
        public static Task<IPAddress> ReflectIPv6Async(
            this IAddressReflectionService service,
            CancellationToken cancellationToken = default)
            => service.ReflectAsync(AddressFamily.InterNetworkV6, cancellationToken);

        /// <summary>
        /// Reflects the public IPv6 address using the specified service with a timeout.
        /// </summary>
        /// <param name="service">
        /// The service to use for reflection.
        /// </param>
        /// <param name="timeout">
        /// The maximum time allowed for the request.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The public IPv6 address of this computer.
        /// </returns>
        public static Task<IPAddress> ReflectIPv6Async(
            this IAddressReflectionService service,
            TimeSpan timeout,
            CancellationToken cancellationToken = default)
            => service.ReflectAsync(AddressFamily.InterNetworkV6, (int)timeout.TotalMilliseconds, cancellationToken);

        /// <summary>
        /// Reflects the public IP address using the specified service.
        /// </summary>
        /// <param name="service">
        /// The service to use for reflection.
        /// </param>
        /// <param name="addressFamily">
        /// The address family to reflect.
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
        public static Task<IPAddress> ReflectAsync(
            this IAddressReflectionService service,
            AddressFamily addressFamily,
            CancellationToken cancellationToken = default)
            => service.ReflectAsync(addressFamily, 0, cancellationToken);

        /// <summary>
        /// Reflects the public IP address using the specified service with a timeout.
        /// </summary>
        /// <param name="service">
        /// The service to use for reflection.
        /// </param>
        /// <param name="addressFamily">
        /// The address family to reflect.
        /// </param>
        /// <param name="timeoutMilliseconds">
        /// The maximum time in milliseconds allowed for the request. Use 0 for no timeout.
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
        /// <exception cref="TimeoutException">
        /// Thrown when the operation exceeds the specified timeout.
        /// </exception>
        public static async Task<IPAddress> ReflectAsync(
            this IAddressReflectionService service,
            AddressFamily addressFamily,
            int timeoutMilliseconds,
            CancellationToken cancellationToken = default)
        {
            if (addressFamily != AddressFamily.InterNetwork &&
                addressFamily != AddressFamily.InterNetworkV6)
                throw new ArgumentException($"Selected address family is not supported - {addressFamily}", nameof(addressFamily));

            using (var timeoutCts = timeoutMilliseconds > 0
                ? new CancellationTokenSource(timeoutMilliseconds)
                : new CancellationTokenSource())
            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token))
            {
                var effectiveToken = linkedCts.Token;

                try
                {
                    if (service is IPublicAddressReflectionService)
                    {
                        var publicService = (IPublicAddressReflectionService)service;

                        using (var responseStream = await publicService.CommunicateAsync(addressFamily, default, effectiveToken).ConfigureAwait(false))
                        {
                            if (object.ReferenceEquals(Stream.Null, responseStream))
                                return null;

                            var addr = await publicService.ParseResponse(responseStream, effectiveToken).ConfigureAwait(false);

                            if (addr == default)
                                return null;

                            return addr;
                        }
                    }
                    else if (service is IStunAddressReflectionService)
                    {
                        var stunService = (IStunAddressReflectionService)service;
                        var response = await stunService.CommunicateAsync(addressFamily, default, default, effectiveToken).ConfigureAwait(false);
                        var result = ParseStunResponse(response);
                        return result.Address;
                    }
                    else
                        throw new ArgumentException($"Unsupported service type '{service?.GetType()}' found.", nameof(service));
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested && timeoutMilliseconds > 0)
                {
                    throw new TimeoutException($"The operation has timed out after {timeoutMilliseconds}ms.");
                }
            }
        }

        /// <summary>
        /// Communicates with the public address reflection service.
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
        /// The response stream from the service.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the service is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the buffer size is less than the default value.
        /// </exception>
        /// <exception cref="ReflectionIPAddressException">
        /// Thrown when no address is found for the host.
        /// </exception>
        /// <exception cref="TimeoutException">
        /// Thrown when the operation exceeds the specified timeout.
        /// </exception>
        public static async Task<Stream> CommunicateAsync(
            this IPublicAddressReflectionService service, AddressFamily addressFamily,
            int bufferSize = Constants.DefaultBufferSize,
            CancellationToken cancellationToken = default)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (bufferSize < Constants.DefaultBufferSize)
                bufferSize = Constants.DefaultBufferSize;
            var targetUri = service.ServiceUri;
            if (targetUri == null)
                throw new ArgumentNullException(nameof(IAddressReflectionService.ServiceUri));
            if (!string.Equals(Uri.UriSchemeHttps, targetUri.Scheme, StringComparison.Ordinal) &&
                !string.Equals(Uri.UriSchemeHttp, targetUri.Scheme, StringComparison.Ordinal))
                throw new ArgumentException($"Selected URI has incompatible scheme - {targetUri.Scheme}", nameof(targetUri));

            var host = targetUri.Host;
            var port = targetUri.Port;
            var pathAndQuery = targetUri.PathAndQuery;

            Socket socket = null;
            NetworkStream networkStream = null;
            SslStream sslStream = null;
            var success = false;

            try
            {
                socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
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

                networkStream = new NetworkStream(socket, ownsSocket: true);
                sslStream = new SslStream(networkStream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
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
                    {
                        success = true;
                        return sslStream;
                    }
                }

                return Stream.Null;
            }
            finally
            {
                if (!success)
                {
                    sslStream?.Dispose();
                    // NetworkStream with ownsSocket:true will dispose the socket,
                    // but if networkStream was never created, dispose socket directly.
                    if (networkStream == null)
                        socket?.Dispose();
                }
            }
        }

        private static bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
            => sslPolicyErrors == SslPolicyErrors.None;

        /// <summary>
        /// Communicates with the STUN service to retrieve the public IP address.
        /// </summary>
        /// <param name="service">
        /// The service to use for reflection.
        /// </param>
        /// <param name="addressFamily">
        /// The address family to reflect.
        /// </param>
        /// <param name="sendTimeoutMilliseconds">
        /// The timeout for sending a request to the service.
        /// </param>
        /// <param name="receiveTimeoutMilliseconds">
        /// The timeout for receiving a response from the service.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The response from the STUN service.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the service is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the send timeout is less than the default value.
        /// </exception>
        /// <exception cref="ReflectionIPAddressException">
        /// Thrown when no address is found for the host.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// Thrown when the data is invalid.
        /// </exception>
        public static async Task<byte[]> CommunicateAsync(
            this IStunAddressReflectionService service, AddressFamily addressFamily,
            int sendTimeoutMilliseconds = Constants.UdpDefaultSendTimeoutMilliseconds,
            int receiveTimeoutMilliseconds = Constants.UdpDefaultReceiveTimeoutMilliseconds,
            CancellationToken cancellationToken = default)
        {
            // This code is quoted from https://forum.dotnetdev.kr/t/ip/11245/5.
            // Thanks to tkm (https://forum.dotnetdev.kr/u/tkm/)
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (sendTimeoutMilliseconds < Constants.UdpDefaultSendTimeoutMilliseconds)
                sendTimeoutMilliseconds = Constants.UdpDefaultSendTimeoutMilliseconds;
            if (receiveTimeoutMilliseconds < Constants.UdpDefaultReceiveTimeoutMilliseconds)
                receiveTimeoutMilliseconds = Constants.UdpDefaultReceiveTimeoutMilliseconds;

            const short stunTypeRequest = 0x0001;
            const short stunTypeResponse = 0x0101;
            const int stunMagicConstant = 0x2112A442;

            var targetUri = service.ServiceUri;
            if (targetUri == null)
                throw new ArgumentNullException(nameof(IAddressReflectionService.ServiceUri));
            if (!string.Equals("stun", targetUri.Scheme, StringComparison.Ordinal))
                throw new ArgumentException($"Selected URI has incompatible scheme - {targetUri.Scheme}", nameof(targetUri));

            var host = targetUri.Host;
            var port = targetUri.Port;

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

            var ipEndPoint = new IPEndPoint(ipAddr, port);

            using (var client = new UdpClient(addressFamily))
            {
                client.Connect(ipEndPoint);

                var req = new byte[20];
                var messageType = (short)ReverseEndianness((ushort)stunTypeRequest);
                var messageParam = stunMagicConstant;

                Buffer.BlockCopy(BitConverter.GetBytes(messageType), 0, req, 0, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(messageParam), 0, req, 4, 4);

                var randomBytes = new byte[12];
                Constants.SharedRandom.Value.NextBytes(randomBytes);
                Array.Copy(randomBytes, 0, req, 8, 12);

                var sendTask = client.SendAsync(req, req.Length);
                if (await Task.WhenAny(sendTask, Task.Delay(sendTimeoutMilliseconds)).ConfigureAwait(false) != sendTask)
                    throw new TimeoutException("The operation has timed out.");

                var receiveTask = client.ReceiveAsync();
                if (await Task.WhenAny(receiveTask, Task.Delay(receiveTimeoutMilliseconds)).ConfigureAwait(false) != receiveTask)
                    throw new TimeoutException("The operation has timed out.");

                var res = await receiveTask.ConfigureAwait(false);
                if ((short)ReverseEndianness((ushort)BitConverter.ToInt16(res.Buffer, 0)) != stunTypeResponse)
                    throw new InvalidDataException();
                if (BitConverter.ToInt32(res.Buffer, 4) != stunMagicConstant)
                    throw new InvalidDataException();
                return res.Buffer.Skip(20).ToArray();
            }
        }

        internal static IPEndPoint ParseStunResponse(byte[] buffer)
        {
            var offset = 0;
            while (offset < buffer.Length)
            {
                if (ReadUInt16(buffer, ref offset) == 0x0001u)
                {
                    ReadUInt16(buffer, ref offset); // size
                    var parsedAddrFamily = ReadUInt16(buffer, ref offset);
                    var port = ReadUInt16(buffer, ref offset);
                    switch (parsedAddrFamily)
                    {
                        case 0x01:
                            var ipv4Address = new byte[4];
                            Array.Copy(buffer, offset, ipv4Address, 0, 4);
                            offset += 4;
                            return new IPEndPoint(new IPAddress(ipv4Address), port);
                        case 0x02:
                            var ipv6Address = new byte[16];
                            Array.Copy(buffer, offset, ipv6Address, 0, 16);
                            offset += 16;
                            return new IPEndPoint(new IPAddress(ipv6Address), port);
                        default:
                            throw new NotSupportedException();
                    }
                }

                var size = ReadUInt16(buffer, ref offset);
                offset += size;
            }

            throw new InvalidDataException();
        }

        private static ushort ReadUInt16(byte[] buffer, ref int offset)
        {
            ushort result = (ushort)((buffer[offset] << 8) | buffer[offset + 1]);
            offset += 2;
            return result;
        }

        private static ushort ReverseEndianness(ushort value)
        {
            return (ushort)((value >> 8) + (value << 8));
        }

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
        /// Reflects the public IP address with detailed information (country, city, ASN, etc.)
        /// using a rich address reflection service such as <see cref="IFConfigService"/>.
        /// </summary>
        /// <param name="service">
        /// A service that implements <see cref="IRichAddressReflectionService"/>.
        /// </param>
        /// <param name="addressFamily">
        /// The address family to reflect.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// Detailed IP address information including geographic and network metadata.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the service is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the selected address family is not supported.
        /// </exception>
        public static async Task<IPAddressInfo> ReflectDetailedAsync(
            this IRichAddressReflectionService service,
            AddressFamily addressFamily = AddressFamily.InterNetwork,
            CancellationToken cancellationToken = default)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (addressFamily != AddressFamily.InterNetwork &&
                addressFamily != AddressFamily.InterNetworkV6)
                throw new ArgumentException($"Selected address family is not supported - {addressFamily}", nameof(addressFamily));

            using (var responseStream = await service.CommunicateAsync(addressFamily, default, cancellationToken).ConfigureAwait(false))
            {
                if (object.ReferenceEquals(Stream.Null, responseStream))
                    return null;

                return await service.ParseDetailedResponse(responseStream, cancellationToken).ConfigureAwait(false);
            }
        }

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
