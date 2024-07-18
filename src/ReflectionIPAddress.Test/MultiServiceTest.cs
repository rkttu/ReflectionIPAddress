using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ReflectionIPAddress.Test
{
    public class MultiServiceTest
    {
        [Fact]
        public void Test_IPv4_Multi()
        {
            var services = new PublicAddressReflectionServices()
                .AddService<IpifyService>()
                .AddService<SeeIPService>()
                .AddService<IP6MeService>()
                .AddService<CurlMyIPService>()
                .AddService<ICanHazIPService>()
                .AddService<IFConfigService>()
                ;

            var address = services.ReflectIPv4Async().Result;
            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        }

        [SkippableFact(typeof(Xunit.SkipException))]
        public void Test_IPv6_Multi()
        {
            var ipv6Available = false;
            foreach (var eachNetworkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                try
                {
                    var ipv6Prop = eachNetworkInterface.GetIPProperties().GetIPv6Properties();

                    if (ipv6Prop == null)
                        continue;
                    if (ipv6Prop.Index <= (-999))
                        continue;

                    ipv6Available = true;
                    break;
                }
                catch (NetworkInformationException) { continue; }
            }

            Skip.IfNot(ipv6Available, "IPv6 is not available.");

            var services = new PublicAddressReflectionServices()
                .AddService<IpifyService>()
                .AddService<SeeIPService>()
                .AddService<IP6MeService>()
                .AddService<CurlMyIPService>()
                .AddService<ICanHazIPService>()
                .AddService<IFConfigService>()
                ;

            var address = services.ReflectIPv6Async().Result;
            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetworkV6, address.AddressFamily);
        }
    }
}
