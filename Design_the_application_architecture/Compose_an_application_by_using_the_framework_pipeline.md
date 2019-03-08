> This chapter should cover:
> - [Add custom request processing modules to the pipeline]()
> - [Add, remove, and configure services used in the application]()
> - [Design and implement middleware]()
> - [Design for kestrel, Http.sys web server and IIS]()
> - [Design and implement startup filters]()

## Add custom request processing modules to the pipeline
## Add, remove, and configure services used in the application
## Design and implement middleware
## Design for kestrel, HTTP.sys web server and IIS
### Kestrel
Kestrel is the web server that's included by default in ASP.NET Core  
Kestrel is supported on all platforms and versions that .NET Core supports

Kestrel support:
 * HTTPS
 * Opaque upgrade used to enable WebSockets
 * Unix sockets for high performance behind Nginx
 * HTTP/2 (except on macOS†)
   * Target framework: .NET Core 2.2 or later
   * Windows Server 2016/Windows 10 or later OR Linux with OpenSSL 1.0.2 or later
   * TLS 1.2

Even if a reverse proxy server isn't required, using a reverse proxy server might be a good choice.

#### How to use Kestrel:
In Program.cs, using `WebHost.CreateDefaultBuilder`
```csharp
public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            // ConfigureKestrel is optional,
            // it is used to provide additional configuration to the Kestrel web server
            .ConfigureKestrel((context, options) =>
            {
                // Set properties and call methods on options
                options.Limits.MaxConcurrentConnections = 100;
                options.Limits.MaxConcurrentUpgradedConnections = 100;
                options.Limits.MaxRequestBodySize = 10 * 1024;
                options.Limits.MinRequestBodyDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                options.Limits.MinResponseDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                options.Listen(IPAddress.Loopback, 5000);
                options.Listen(IPAddress.Loopback, 5001, listenOptions =>
                {
                    listenOptions.UseHttps("testCert.pfx", "testPassword");
                });
            });
}
```
Or with `WebHostBuilder`
```csharp
public static void Main(string[] args)
{
    var host = new WebHostBuilder()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseKestrel() // If the app doesn't call CreateDefaultBuilder to set up the host, call UseKestrel before calling ConfigureKestrel
        .UseIISIntegration()
        .UseStartup<Startup>()
        .ConfigureKestrel((context, options) =>
        {
            // Set properties and call methods on options
        })
        .Build();

    host.Run();
}
```

### HTTP.sys

## Design and implement startup filters