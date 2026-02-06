# ReflectionIPAddress

[![NuGet Version](https://img.shields.io/nuget/v/ReflectionIPAddress)](https://www.nuget.org/packages/ReflectionIPAddress/) [![NuGet Downloads](https://img.shields.io/nuget/dt/ReflectionIPAddress)](https://www.nuget.org/packages/ReflectionIPAddress/) ![Build Status](https://github.com/rkttu/ReflectionIPAddress/actions/workflows/dotnet.yml/badge.svg) [![GitHub Sponsors](https://img.shields.io/github/sponsors/rkttu)](https://github.com/sponsors/rkttu/)

This is a library that checks the external IP address of a running PC, acquires a wildcard domain, and searches IP band information.

This library implements the simultaneous queries of several free services (e.g. ipify.org, ip6.me, etc.) that provide a service that allows you to look up the other party's external IP address in a similar way without separate authentication, thereby ensuring service stability guaranteed.

Also, this library explicitly uses direct TCP socket connections and SSL connections to use IPv4 or IPv6 communication.

## Features

- **Multiple service providers** — Queries multiple free services simultaneously and returns the fastest successful response
- **STUN protocol support** — Get your public IP via Google STUN servers (UDP-based, faster than HTTP)
- **IPv4 and IPv6** — Explicit control over which address family to use
- **Timeout support** — Per-service timeout to prevent slow services from blocking results
- **Consensus API** — Query all services and compare results for security-sensitive applications
- **Rich IP information** — Get country, city, ASN, time zone, and coordinates via IFConfig service
- **Wildcard domain** — Convert IP addresses to sslip.io wildcard domains
- **Dependency Injection** — Built-in support for `Microsoft.Extensions.DependencyInjection` (net6.0+)
- **SourceLink enabled** — Step into library source code during debugging

## Supported Services

| Service | Protocol | Type |
| --------- | ---------- | ------ |
| `CloudflareTraceService` | HTTPS | HTTP |
| `IpifyService` | HTTPS | HTTP |
| `SeeIPService` | HTTPS | HTTP |
| `IP6MeService` | HTTPS | HTTP |
| `CurlMyIPService` | HTTPS | HTTP |
| `ICanHazIPService` | HTTPS | HTTP |
| `IFConfigService` | HTTPS | HTTP (supports rich info) |
| `GoogleStunService` | UDP | STUN |
| `GoogleStun1Service` ~ `GoogleStun4Service` | UDP | STUN |
| `DistributedGoogleStunService` | UDP | STUN |

## Update

### v0.7.0

- Multi-targeting: `netstandard2.0`, `net6.0`, `net8.0`
- Added `ReflectAllAsync` / `GetConsensusAddress` for consensus-based IP resolution
- Added per-service timeout support (`TimeSpan` overloads)
- Added `IRichAddressReflectionService` and `IPAddressInfo` for detailed geo/network info
- Added DI integration (`services.AddReflectionIPAddress()`) for net6.0+
- SourceLink enabled for debugging
- Fixed resource leaks in socket/stream handling
- Fixed Regex caching in CloudflareTraceService
- Updated `System.Text.Json` to 9.0.1 (security fix)

### v0.6.1

- Added a new service, CloudflareTraceService, to support Cloudflare Trace protocol to get the public IP address.

### v0.6.0

- Starting with v0.6, the library supports STUN protocol to get the public IP address.
- Added a new service, GoogleStunService, to support STUN protocol to get the public IP address.

## Requirements

- Requires a platform with .NET Standard 2.0 or later
  - Supported .NET Version: .NET Core 2.0+, .NET 5+, .NET Framework 4.6.1+, Mono 5.4+, UWP 10.0.16299+, Unity 2018.1+

## How to use

### Basic Usage

```csharp
using ReflectionIPAddress;

var services = new PublicAddressReflectionServices()
    .AddService<CloudflareTraceService>()
    .AddService<IpifyService>()
    .AddService<SeeIPService>()
    .AddService<IP6MeService>()
    .AddService<CurlMyIPService>()
    .AddService<ICanHazIPService>()
    .AddService<IFConfigService>()
    .AddService<GoogleStunService>();

// Returns the IP address by checking for the fastest successful response among the specified services.
var ipv4Address = await services.ReflectIPv4Async();
var sslipDomain = ipv4Address.ToSSLIPDomain();

Console.WriteLine($"IPv4 Address: {ipv4Address}, SSLIP Domain: {sslipDomain}");
```

### With Timeout

```csharp
// Each service gets 5 seconds to respond
var ipv4Address = await services.ReflectIPv4Async(TimeSpan.FromSeconds(5));
```

### Consensus-Based Resolution

```csharp
// Query all services and get the most common result
var allResults = await services.ReflectAllIPv4Async(
    perServiceTimeout: TimeSpan.FromSeconds(10));

var consensusIp = allResults.GetConsensusAddress();
Console.WriteLine($"Consensus IP: {consensusIp} (from {allResults.Count} services)");
```

### Rich IP Information

```csharp
// Get detailed information including country, city, ASN, etc.
var ifconfig = new IFConfigService();
var info = await ifconfig.ReflectDetailedAsync();

Console.WriteLine($"IP: {info.Address}");
Console.WriteLine($"Country: {info.Country} ({info.CountryISO})");
Console.WriteLine($"City: {info.City}");
Console.WriteLine($"ASN: {info.ASN} ({info.ASNOrganization})");
Console.WriteLine($"TimeZone: {info.TimeZone}");
```

### Dependency Injection (net6.0+)

```csharp
// In Program.cs or Startup.cs
services.AddReflectionIPAddress(); // Registers all built-in services

// Or configure specific services
services.AddReflectionIPAddress(builder =>
{
    builder.AddService<CloudflareTraceService>();
    builder.AddService<GoogleStunService>();
});

// Then inject and use
public class MyService
{
    private readonly PublicAddressReflectionServices _reflectionServices;

    public MyService(PublicAddressReflectionServices reflectionServices)
        => _reflectionServices = reflectionServices;

    public Task<IPAddress> GetMyIpAsync()
        => _reflectionServices.ReflectIPv4Async();
}
```

## License

This library follows Apache-2.0 license. See [LICENSE](./LICENSE) file for more information.
