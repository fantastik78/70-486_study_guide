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
## Choose when to use .NET standard libraries
