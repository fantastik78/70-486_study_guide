> This chapter should cover:
> - [Manage configuration sources, including XML, JSON, and INI files]()
> - [Manage environment variables]()
> - [Implement Option objects]()
> - [Implement multiple environments using files and hierarchical structure]()
> - [Manage sensitive configuration]()
> - [React to runtime configuration changes]()
> - [Implement a custom configuration source]()
> - [Secure configuration by using Azure Key Vault]()
> - [Use the Secret Manager tool in development to keep secrets out of your code for configuration values]()

## Manage configuration sources, including XML, JSON, and INI files

Call ConfigureAppConfiguration when building the host to specify the app's configuration providers in addition to those added automatically by CreateDefaultBuilder:

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            // Configuration supplied to the app in ConfigureAppConfiguration is available during the app's startup, including Startup.ConfigureServices
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                
                config.AddJsonFile("config.json", optional: false, reloadOnChange: false);
                config.AddJsonFile($"config.{env.EnvironmentName}.json", optional: false, reloadOnChange: false);
                config.AddXmlFile("config.xml", optional: false, reloadOnChange: false);
                config.AddIniFile("config.ini", optional: true, reloadOnChange: true);

                config.AddEFConfiguration(options => options.UseInMemoryDatabase("InMemoryDb"));
                config.AddCommandLine(args);
            })
            .UseStartup<Startup>();
}
```

Configuration files are overwriting already existing values.

## Manage environment variables

ASP.NET Core reads the environment variable ASPNETCORE_ENVIRONMENT at app startup and stores the value in IHostingEnvironment.EnvironmentName.

You can set ASPNETCORE_ENVIRONMENT to any value, but three values are supported by the framework: Development, Staging, and Production. If ASPNETCORE_ENVIRONMENT isn't set, it defaults to Production.

### To set the environment:

* Windows (at machine levle)

CMD:
```cmd
setx ASPNETCORE_ENVIRONMENT Development /M
```

PowerShell:
```PowerShell
[Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development", "Machine")
```
* web.config

```xml
<PropertyGroup>
  <EnvironmentName>Development</EnvironmentName>
</PropertyGroup>
```

## Implement Option objects
## Implement multiple environments using files and hierarchical structure

### Using IHostingEnvironment

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    if (env.IsProduction() || env.IsStaging() || env.IsEnvironment("Staging_2"))
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    //...
}
```
### Startup class conventions

When an ASP.NET Core app starts, the Startup class bootstraps the app. The app can define separate Startup classes for different environments (for example, StartupDevelopment), and the appropriate Startup class is selected at runtime. The class whose name suffix matches the current environment is prioritized. If a matching Startup{EnvironmentName} class isn't found, the Startup class is used.

```csharp
// Startup class to use in the Development environment
public class StartupDevelopment
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ...
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        // ...
    }
}

// Startup class to use in the Production environment
public class StartupProduction
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ...
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        // ...
    }
}

// Fallback Startup class
// Selected if the environment doesn't match a Startup{EnvironmentName} class
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ...
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        // ...
    }
}
```

Use the UseStartup(IWebHostBuilder, String) overload that accepts an assembly name:
```csharp
public static void Main(string[] args)
{
    CreateWebHostBuilder(args).Build().Run();
}

public static IWebHostBuilder CreateWebHostBuilder(string[] args)
{
    var assemblyName = typeof(Startup).GetTypeInfo().Assembly.FullName;

    return WebHost.CreateDefaultBuilder(args)
        .UseStartup(assemblyName);
}
```

### Startup method conventions

The two Startup methods, Configure and ConfigureServices, are read by the framework application initializer if, and only IF, there are no customized methods that match the mode the application is running onto. Which means that you may change the methods names to the following formats, respectively:

* Configure{EnvironmentName}
* Configure{EnvironmentName}Services

For example: 

```csharp
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        StartupConfigureServices(services);
    }

    public void ConfigureDevelopmentServices(IServiceCollection services)
    {
        StartupConfigureServices(services);
    }

    private void StartupConfigureServices(IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy();
        app.UseMvc();
    }

    public void ConfigureDevelopment(IApplicationBuilder app, IHostingEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            throw new Exception("Not Development.");
        }

        app.UseDeveloperExceptionPage();
        app.UseHsts();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy();
        app.UseMvc();
    }
}
```

## Manage sensitive configuration
## React to runtime configuration changes
## Implement a custom configuration source
## Secure configuration by using Azure Key Vault
## Use the Secret Manager tool in development to keep secrets out of your code for configuration values