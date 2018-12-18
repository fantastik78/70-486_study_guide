# Routing

ASP.NET Routing is a pattern matching system, it is at core of every ASP.NET MVC request.
The ASP.NET Routing module is responsible for mapping incoming browser requests to particular MVC controller actions.

ASP.NET Routing is setup in two places:
- in the Web.config file:
  - system.web.httpModules section
  - system.web.httpHandlers section
  - system.webserver.modules section
  - system.webserver.handlers section
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
            "Default", // Unique name
            "{controller}/{action}/{id}", // URL pattern
            new { controller = "Home", action = "Index", id = @"\d+" } // Defaults and Constraints
          );
        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}

```
