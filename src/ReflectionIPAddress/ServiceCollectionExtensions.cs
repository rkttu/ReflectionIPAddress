using System;
using System.Collections.Generic;
using System.Linq;

#if NET8_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#endif

namespace ReflectionIPAddress
{
#if NET8_0_OR_GREATER
    /// <summary>
    /// Extension methods for integrating ReflectionIPAddress with Microsoft.Extensions.DependencyInjection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds IP reflection services to the dependency injection container.
        /// </summary>
        /// <param name="services">
        /// The service collection to add to.
        /// </param>
        /// <param name="configure">
        /// An optional action to configure which reflection services to register.
        /// If not specified, all built-in services are registered.
        /// </param>
        /// <returns>
        /// The service collection for chaining.
        /// </returns>
        public static IServiceCollection AddReflectionIPAddress(
            this IServiceCollection services,
            Action<ReflectionIPAddressBuilder> configure = null)
        {
            var builder = new ReflectionIPAddressBuilder();

            if (configure != null)
            {
                configure(builder);
            }
            else
            {
                builder.AddAllBuiltInServices();
            }

            services.TryAddSingleton(sp =>
            {
                var reflectionServices = new PublicAddressReflectionServices();
                foreach (var service in builder.Services)
                    reflectionServices.Add(service);
                return reflectionServices;
            });

            return services;
        }
    }

    /// <summary>
    /// Builder for configuring which IP reflection services to register.
    /// </summary>
    public sealed class ReflectionIPAddressBuilder
    {
        private readonly List<IAddressReflectionService> _services = new List<IAddressReflectionService>();

        /// <summary>
        /// Gets the registered services.
        /// </summary>
        internal IReadOnlyList<IAddressReflectionService> Services => _services;

        /// <summary>
        /// Adds a specific reflection service.
        /// </summary>
        /// <typeparam name="TService">
        /// The type of service to add.
        /// </typeparam>
        /// <returns>
        /// This builder for chaining.
        /// </returns>
        public ReflectionIPAddressBuilder AddService<TService>()
            where TService : IAddressReflectionService, new()
        {
            if (!_services.Any(s => s.GetType() == typeof(TService)))
                _services.Add(new TService());
            return this;
        }

        /// <summary>
        /// Adds all built-in HTTP and STUN services.
        /// </summary>
        /// <returns>
        /// This builder for chaining.
        /// </returns>
        public ReflectionIPAddressBuilder AddAllBuiltInServices()
        {
            AddService<CloudflareTraceService>();
            AddService<IpifyService>();
            AddService<SeeIPService>();
            AddService<IP6MeService>();
            AddService<CurlMyIPService>();
            AddService<ICanHazIPService>();
            AddService<IFConfigService>();
            AddService<GoogleStunService>();
            return this;
        }

        /// <summary>
        /// Adds all built-in HTTP-only services (excludes STUN).
        /// </summary>
        /// <returns>
        /// This builder for chaining.
        /// </returns>
        public ReflectionIPAddressBuilder AddHttpServices()
        {
            AddService<CloudflareTraceService>();
            AddService<IpifyService>();
            AddService<SeeIPService>();
            AddService<IP6MeService>();
            AddService<CurlMyIPService>();
            AddService<ICanHazIPService>();
            AddService<IFConfigService>();
            return this;
        }

        /// <summary>
        /// Adds all built-in STUN services.
        /// </summary>
        /// <returns>
        /// This builder for chaining.
        /// </returns>
        public ReflectionIPAddressBuilder AddStunServices()
        {
            AddService<GoogleStunService>();
            AddService<GoogleStun1Service>();
            AddService<GoogleStun2Service>();
            AddService<GoogleStun3Service>();
            AddService<GoogleStun4Service>();
            AddService<DistributedGoogleStunService>();
            return this;
        }
    }
#endif
}
