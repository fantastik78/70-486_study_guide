> This chapter should cover:
> - [Create custom middleware and inject it into the pipeline]()
> - [Implement MVC filters and controller factories]()
> - [Control application behavior by using action results, model binders, and route handlers]()
> - [Inject services into a view]()

## Create custom middleware and inject it into the pipeline

Create a C# class:
```csharp
public class MyMiddleware
{
    private readonly RequestDelegate _requestDelegate;

    public MyMiddleware(RequestDelegate requestDelegate)
    {
        _requestDelegate = requestDelegate;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.Headers.Add("Hello-World", "Hello World!");
        await _requestDelegate.Invoke(context);
    }
}
```

The common practice is to create a middleware extension.

```csharp
public static class MyMiddlewareExtensions
{
    public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MyMiddleware>();
    }
}
```

Add the middelware in the pipeline using the extension.
Go to Startup.cs, and add the middleware in the Configure method:
```csharp
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            // Use of the MyMiddleware in the app pipeline
            app.UseMyMiddleware();

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvc();
        }
```

## Implement MVC filters and controller factories
## Control application behavior by using action results, model binders, and route handlers
## Inject services into a view

Create your service (interface  & class):
```csharp
public interface IMyService
{
    string SayHelloWorld();
}

public class MyService : IMyService
{
    public string SayHelloWorld()
    {
        return "Hello world from service";
    }
}
```
In you view file (.cshmtl), reference you service using `@using`  and then use the keyword `@inject` inside you cshtml file:
```cshtml
@page
@using HelloWorld.Services
@model IndexModel
@inject IMyService MyService
@{
    ViewData["Title"] = "Home page";
}

<div class="page-header">
    <h1>@Model.Message</h1>
    <!-- Call you service preceded by @ -->
    <p>@MyService.SayHelloWorld()</p>
</div>
<div>
    <hello-world />
</div>

```