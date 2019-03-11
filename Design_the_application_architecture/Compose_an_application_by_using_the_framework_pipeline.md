# Compose an application by using the framework pipeline

> This chapter should cover:
> - [Add custom request processing modules to the pipeline](#add-custom-request-processing-modules-to-the-pipeline)
> - [Add, remove, and configure services used in the application](#add-remove-and-configure-services-used-in-the-application)
> - [Design and implement middleware](#design-and-implement-middleware)
> - [Design for kestrel, Http.sys web server and IIS](#design-for-kestrel-httpsys-web-server-and-iis)
> - [Design and implement startup filters](#design-and-implement-startup-filters)

## Add custom request processing modules to the pipeline
## Add, remove, and configure services used in the application
## Design and implement middleware

Middleware is generally encapsulated in a class and exposed with an extension method.  
The middleware do not required to implement any interface or inherit from any class. However, it does need to have a specific constructor and a method with a specific signature.

Constructor:
```csharp
public MyMiddleware(RequestDelegate next, IOptions<MyMiddlewareOptions> options)
{
    _options = options.Value;
    _next = next;
}
```

Invoke Method:
```csharp
public async Task Invoke(HttpContext httpContext)
{
    await _next.Invoke(httpContext);
}
```

Extension:
```csharp
public static class MyMiddlewareExtensions
{
    public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder app, MyMiddlewareOptions options)
    {
        return app.UseMiddleware<MyMiddleware>(Options.Create(options));
    }
}
```

Use in Statup.cs
```csharp
app.UseMyMiddleware(new MyMiddlewareOptions {
    // Options 
});
```

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
HTTP.sys is an alternative to Kestrel server and offers some features that Kestrel doesn't provide.

> ![Note](https://img.shields.io/badge/Info-Important-yellow.svg)  
> HTTP.sys isn't compatible with the ASP.NET Core Module and can't be used with IIS or IIS Express.  
> HTTP.sys server **doesn't work in a reverse proxy configuration with IIS**.

HTTP.sys supports:
 * Windows Authentication
 * Port sharing
 * HTTPS with Server Name Indication (SNI is an extension to the TLS computer networking protocol)
 * HTTP/2 over TLS (Windows 10 or later)
 * Direct file transmission
 * Response caching
 * WebSockets (Windows 8 or later)

 HTTP.sys is useful for deployments where a feature not available in Kestrel, such as Windows Authentication
 
#### How to use HTTP.sys:

1. A package reference to Microsoft.AspNetCore.Server.HttpSys  
**A package reference in the project file isn't required when using the Microsoft.AspNetCore.App metapackage**

2. Use of UseHttpSys extension method when building Web Host
```csharp
public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .UseHttpSys(options =>
        {
            // The following options are set to default values.
            // Additional HTTP.sys configuration is handled through registry settings.
            options.Authentication.Schemes = AuthenticationSchemes.None;
            options.Authentication.AllowAnonymous = true;
            options.MaxConnections = null;
            options.MaxRequestBodySize = 30000000;
            options.UrlPrefixes.Add("http://localhost:5000");
        });
```

> ![Important](https://img.shields.io/badge/Info-Warning-orange.svg)  
> Top-level wildcard bindings (http://*:80/ and http://+:80) **should not be used**.  
> Top-level wildcard bindings create app security vulnerabilities.

### IIS 
#### Enable the IISIntegration components
##### In-process hosting model

> ![Note](https://img.shields.io/badge/Info-Important-yellow.svg)  
> **Performance tests indicate that hosting a .NET Core app in-process delivers significantly higher request throughput compared to hosting the app out-of-process and proxying requests to Kestrel server.**

Calls the UseIIS method to boot the CoreCLR and host the app inside of the IIS worker process

##### Out-of-process hosting model

CreateDefaultBuilder calls the UseIISIntegration method. UseIISIntegration configures Kestrel to listen on the dynamic port at the localhost IP address (127.0.0.1)

#### IIS options
##### In-process hosting model

To configure IIS Server options, include a service configuration for IISServerOptions in ConfigureServices

```csharp
services.Configure<IISServerOptions>(options => 
{
    // Options
});
```

##### Out-of-process hosting model

To configure IIS options, include a service configuration for IISOptions in ConfigureServices.

```csharp
services.Configure<IISOptions>(options => 
{
    // Options
});
```

## Design and implement startup filters

Use IStartupFilter to configure middleware at the beginning or end of an app's Configure middleware pipeline.  

```csharp
public class MyStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            builder.UseMiddleware<MyMiddleware>();
            next(builder);
        };
    }
}
```

```csharp
WebHost.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<IStartupFilter, 
            MyStartupFilter>();
    })
    .UseStartup<Startup>()
    .Build();
```