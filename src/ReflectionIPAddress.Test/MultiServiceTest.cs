using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ReflectionIPAddress.Test
{
    public class MultiServiceTest
    {
        [Fact]
        public async Task Test_IPv4_Multi()
        {
            var services = new PublicAddressReflectionServices()
                .AddService<IpifyService>()
                .AddService<SeeIPService>()
                .AddService<IP6MeService>()
                .AddService<CurlMyIPService>()
                .AddService<ICanHazIPService>()
                .AddService<IFConfigService>()
                .AddService<GoogleStunService>()
                ;

            var address = await services.ReflectIPv4Async();
            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        }

        [SkippableFact(typeof(Xunit.SkipException))]
        public async Task Test_IPv6_Multi()
        {
            try
            {
                IpifyService testService = new IpifyService();
                await testService.ReflectIPv6Async();
            }
            catch (SocketException)
            {
                Skip.If(true, "This system does not support IPv6.");
            }
            catch (ReflectionIPAddressException)
            {
                Skip.If(true, "This system does not support IPv6.");
            }

            var services = new PublicAddressReflectionServices()
                .AddService<IpifyService>()
                .AddService<SeeIPService>()
                .AddService<IP6MeService>()
                .AddService<CurlMyIPService>()
                .AddService<ICanHazIPService>()
                .AddService<IFConfigService>()
                .AddService<GoogleStunService>()
                ;

            var address = await services.ReflectIPv6Async();
            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetworkV6, address.AddressFamily);
        }
    }
}
