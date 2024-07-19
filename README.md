# ReflectionIPAddress

[![NuGet Version](https://img.shields.io/nuget/v/ReflectionIPAddress)](https://www.nuget.org/packages/ReflectionIPAddress/) ![Build Status](https://github.com/rkttu/ReflectionIPAddress/actions/workflows/dotnet.yml/badge.svg) [![GitHub Sponsors](https://img.shields.io/github/sponsors/rkttu)](https://github.com/sponsors/rkttu/)

This is a library that checks the external IP address of a running PC, acquires a wildcard domain, and searches IP band information.

This library implements the simultaneous queries of several free services (e.g. ipify.org, ip6.me, etc.) that provide a service that allows you to look up the other party's external IP address in a similar way without separate authentication, thereby ensuring service stability guaranteed.

Also, this library explicitly uses direct TCP socket connections and SSL connections to use IPv4 or IPv6 communication.

## Update

- Starting with v0.6, the library supports STUN protocol to get the public IP address.

## Requirements

- Requires a platform with .NET Standard 2.0 or later
  - Supported .NET Version: .NET Core 2.0+, .NET 5+, .NET Framework 4.6.1+, Mono 5.4+, UWP 10.0.16299+, Unity 2018.1+

## How to use

```csharp
using ReflectionIPAddress;

...

var services = new PublicAddressReflectionServices()
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

//var ipv6Address = await services.ReflectIPv6Async();

Console.Out.WriteLine($"IPv4 Address: {ipv4Address}, SSLIP Domain: {sslipDomain}");")
```

## License

This library follows Apache-2.0 license. See [LICENSE](./LICENSE) file for more information.
