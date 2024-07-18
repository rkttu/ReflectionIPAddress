using System.Net.Sockets;

namespace ReflectionIPAddress.Test
{
    public class ServiceTest
    {
        [Fact]
        public async Task Test_IpifyService()
        {
            var services = new PublicAddressReflectionServices().AddService<IpifyService>();
            var address = await services.ReflectIPv4Async();

            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        }

        [Fact]
        public async Task Test_SeeIPService()
        {
            var services = new PublicAddressReflectionServices().AddService<SeeIPService>();
            var address = await services.ReflectIPv4Async();

            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        }

        [Fact]
        public async Task Test_IP6MeService()
        {
            var services = new PublicAddressReflectionServices().AddService<IP6MeService>();
            var address = await services.ReflectIPv4Async();

            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        }

        [Fact]
        public async Task Test_CurlMyIPService()
        {
            var services = new PublicAddressReflectionServices().AddService<CurlMyIPService>();
            var address = await services.ReflectIPv4Async();

            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        }

        [Fact]
        public async Task Test_ICanHazIPService()
        {
            var services = new PublicAddressReflectionServices().AddService<ICanHazIPService>();
            var address = await services.ReflectIPv4Async();

            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        }

        [Fact]
        public async Task Test_IFConfigService()
        {
            var services = new PublicAddressReflectionServices().AddService<IFConfigService>();
            var address = await services.ReflectIPv4Async();

            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        }
    }
}
