> This chapter should cover:
> - Define a route to handle a URL pattern
> - Apply route constraints
> - Ignore URL patterns
> - Add custom route parameters
> - Define areas
> - Define routes that interoperate with Single Page Application frameworks such as Angular

# Routing

ASP.NET Routing is a pattern matching system, it is at core of every ASP.NET MVC request.
The ASP.NET Routing module is responsible for mapping incoming browser requests to particular MVC controller actions.

ASP.NET Routing is setup in two places:
- in the Global.asax file:
  - a route table is created during the Application Start event

```csharp
namespace MvcApplication1
{
  public class MvcApplication : System.Web.HttpApplication
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.MapRoute(
        "Default",                                              // Unique Route name
        "{controller}/{action}/{id}",                           // URL pattern with parameters
        new { controller = "Home", action = "Index", id = "" }  // Defaults and Constraints
      );
    }

    protected void Application_Start()
    {
      RegisterRoutes(RouteTable.Routes);
    }
  }
}
```
- in the Controllers (MVC5):
  - using Route attributes <br/>(This route attribute will run your About method any time a request comes in with /about or /home/about as the URL.)
```csharp
namespace MvcApplication1
{
  public class MvcApplication : System.Web.HttpApplication
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.MapMvcAttributeRoutes();
    }
      
    protected void Application_Start()
    {
      RegisterRoutes(RouteTable.Routes);
    }
  }
}
```
```csharp
[Route("home/{action}")]
public class HomeController: Controller
{
  [Route("home")][Route("home/index")]
  public ActionResult Index()
  {
		return View();
	}
	public ActionResult About()
  {
		return View();
	}
	public ActionResult Contact()
  {
		return View();
	}
}
```

## Attribute Routing

## Traditional Routing

Add any custom route to the Global.asax file.

Like for example a route named Blog, which handles requests that look like /Archive/entry date:
```csharp
routes.MapRoute(
  "Blog",
  "Archive/{entryDate}",
  new { controller = "Archive", action = "Entry" }
);
```
