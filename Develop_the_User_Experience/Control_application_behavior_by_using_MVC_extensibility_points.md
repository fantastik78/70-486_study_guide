> This chapter should cover:
> - [Create custom middleware and inject it into the pipeline]()
> - [Implement MVC filters and controller factories]()
> - [Control application behavior by using action results, model binders, and route handlers]()
> - [Inject services into a view]()

## Create custom middleware and inject it into the pipeline
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