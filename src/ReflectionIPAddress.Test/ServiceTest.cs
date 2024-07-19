using System.Net.Sockets;

namespace ReflectionIPAddress.Test
{
    public class ServiceTest
    {
        [Fact]
        public async Task Test_CloudflareTraceService()
        {
            var services = new PublicAddressReflectionServices().AddService<CloudflareTraceService>();
            var address = await services.ReflectIPv4Async();

            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        }

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
        public async Task Test_GoogleStunService()
        {
            var services = new PublicAddressReflectionServices().AddService<GoogleStunService>();
            var address = await services.ReflectIPv4Async();

            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        }

        [Fact]
        public async Task Test_GoogleStun1Service()
        {
            var services = new PublicAddressReflectionServices().AddService<GoogleStun1Service>();
            var address = await services.ReflectIPv4Async();

            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        }

        [Fact]
        public async Task Test_GoogleStun2Service()
        {
            var services = new PublicAddressReflectionServices().AddService<GoogleStun2Service>();
            var address = await services.ReflectIPv4Async();

            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        }

        [Fact]
        public async Task Test_GoogleStun3Service()
        {
            var services = new PublicAddressReflectionServices().AddService<GoogleStun3Service>();
            var address = await services.ReflectIPv4Async();

            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        }

        [Fact]
        public async Task Test_GoogleStun4Service()
        {
            var services = new PublicAddressReflectionServices().AddService<GoogleStun4Service>();
            var address = await services.ReflectIPv4Async();

            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        }

        [Fact]
        public async Task Test_DistributedGoogleStunService()
        {
            var services = new PublicAddressReflectionServices().AddService<DistributedGoogleStunService>();
            var address = await services.ReflectIPv4Async();

            Assert.NotNull(address);
            Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        }
    }
}
