> This chapter should cover:
> - [Plan data access]()
> - [Plan for separation of concerns, appropriate use of models, views, controllers, components, and service dependency injection]()
> - [Choose between client-side and server-side processing]()
> - [Design for scalability]()
> - [Choose between ASP.NET Core and ASP.NET]()
> - [Choose when to use .NET standard libraries]()

## Plan data access
## Plan for separation of concerns, appropriate use of models, views, controllers, components, and service dependency injection
### Service dependency injection

Code to configure (or register) services is added to the Startup.ConfigureServices
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Transient objects are always different
        services.AddTransient<ITransientDependency, TransientDependency>();

        // Scoped objects are the same within a request but different across requests
        services.AddScoped<IScopedDependency, ScopedDependency>();
        
        // Singleton objects are the same for every object and every request regardless of whether an Operation instance is provided in ConfigureServices
        services.AddSingleton<ISingletonDependency, SingletonDependency>();
    }
}
```

Usage in a HelloWorldModel page
```csharp
public class HelloWorldModel : PageModel
{
    private readonly IScopedDependency _scopedDependency;

    public IndexModel(
        IOperationTransient transientDependency,
        IScopedDependency scopedDependency, 
        ISingletonDependency singletonDependency)
    {
        _scopedDependency = scopedDependency;
    }

    public async Task OnGetAsync()
    {
        await _scopedDependency.HelloWorld();
    }
}
```

Dependency injection inside a MVC Controller
```csharp
public class HomeController : Controller
{
    private readonly IScopedDependency _scopedDependency;

    public HomeController(IScopedDependency scopedDependency)
    {
        _scopedDependency = scopedDependency;
    }

    public IActionResult Index()
    {
        ViewData["Data"] = _scopedDependency.HelloWorld();
        return View();
    }
}
```

Dependency injection inside a MVC View
```cshtml
@using HelloWorld.Services
@inject IScopedDependency ScopedDependency
<!DOCTYPE html>
<html>
<head>
    <title>Hello World</title>
</head>
<body>
    <div>
        <h1>Hello World</h1>
        <span>@ScopedDependency.HelloWorld()</span>
    </div>
</body>
</html>
```

```csharp
using System.Linq;
using ViewInjectSample.Interfaces;

namespace HelloWorld.Services
{
    public class ScopedDependency : IScopedDependency
    {
        private readonly IHelloWorldRepository _helloWorldRepository;

        public StatisticsService(IHelloWorldRepository helloWorldRepository)
        {
            _helloWorldRepository = helloWorldRepository;
        }

        public string HelloWorld()
        {
            return _helloWorldRepository.HelloWorld();
        }
    }
}
```

## Choose between client-side and server-side processing
## Design for scalability
## Choose between ASP.NET Core and ASP.NET
### Framework selection:
* ASP.NET Core
  * Cross-Plateform needs
  * Microservices
  * Containers
  * Need of high-performance
  * Need for scalability
  * Side-by-side versioning
  * Open source

Razor Pages is the recommended approach to create a Web UI as of ASP.NET Core 2.x.

* ASP.NET 4.x
  * Need to use .NET libraries or NuGet package not available for .NET Core
  * Need to use .NET technologies not available for .NET Core
    * ASP.NET Web Forms
    * ASP.NET Web Pages
    * WCF service implementation (aka server side implementation)
    * Windows Workflow Foundation / Workflow Services / WCF Data Services
    * Language support: Visual Basic
  * Need to use a platform that doesn't support .NET Core

## Choose when to use .NET standard libraries

.NET Standard is a specification of APIs.  
The purpose of .NET Standard is to share code between runtimes.  
When you want to share code between different runtimes in the .NET Ecosystem, use .NET Standard.

* .NET Standard is a set of curated APIs, picked by Microsoft, PCLS are not.
  * The APIs that a PCL contains is dependent on the platforms that you choose to target when you create a PCL. This makes a PCL only sharable for the specific targets that you choose.

* .NET Standard is platform-agnostic, it can run anywhere, on Windows, Mac, Linux and so on.
  * PCLs can also run cross-platform, but they have a more limited reach. PCLs can only target a limited set of platform
