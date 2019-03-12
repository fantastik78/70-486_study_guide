> This chapter should cover:
> - [Design and implement pages by using Razor Pages](#design-and-implement-pages-by-using-razor-pages)
> - [Design and implement layouts to provide visual structure](#design-and-implement-layouts-to-provide-visual-structure)
> - [Define and render optional and required page sections](#define-and-render-optional-and-required-page-sections)
> - [Implement partial views and view components for reuse in different areas of the application](#implement-partial-views-and-view-components-for-reuse-in-different-areas-of-the-application)
> - [Create and use tag and HTML helpers to simplify markup](#create-and-use-tag-and-html-helpers-to-simplify-markup)

## Design and implement pages by using Razor Pages
### Razor Page

```cshtml
@page
@using MyModels.Pages
@model MyModel

<h2>Separate page model</h2>
<p>
    @Model.Message
</p>
```

`@page` must be the first Razor directive on a page, it differenciates it to ASP.NET MVC Views.

```csharp
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace MyModels.Pages
{
    public class MyModel : PageModel
    {
        public string Message { get; private set; } = "PageModel in C#";

        public void OnGet()
        {
            Message += $" Server time is { DateTime.Now }";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Business Logic
            return RedirectToPage("/Index");
        }
    }
}
```
The PageModel class allows separation of the logic of a page from its presentation. It defines page handlers for requests sent to the page and the data used to render the page.  
This separation allows you to manage page dependencies through dependency injection

## Design and implement layouts to provide visual structure

### _Layout.cshtml
* Define a top level template for views in the app
* Not required

Exemple of _Layout.cshtml
```cshtml
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Hellow World</title>
</head>
<body>
    <div class="container body-content">
        @RenderBody()
        <footer>
            <p>&copy; 2018 - HelloWorld</p>
        </footer>
    </div>
</body>
</html>
```
By default, every layout must call RenderBody. Wherever the call to RenderBody is placed, the contents of the view will be rendered.

Individual views can specify a layout by setting this property:
```cshtml
@{
    Layout = "_Layout";
}
```

To use the same layout on all pages, we can specify the default layout in the _ViewStart.cshtml.  
If a view specify a Layout, this will overide the _Layout.cshtml

### _ViewStart.cshtml

The _ViewStart.cshtml file contains code that executes after the code in any content page in the same folder or any child folders.

It provides a convenient location to specify the layout file for all content pages that are affected by it.

_ViewStart.cshtml is hierarchical. If a _ViewStart.cshtml file is defined in the view or pages folder, it will be run after the one defined in the root of the Pages (or Views) folder (if any).

### _ViewImports.cshtml

The purpose of the _ViewImports.cshtml file is to provide a mechanism to make directives available to Razor pages globally so that you don't have to add them to pages individually.

Example:
```cshtml
@using WebApplication1
@using WebApplication1.Models
@using WebApplication1.Models.AccountViewModels
@using WebApplication1.Models.ManageViewModels
@using Microsoft.AspNetCore.Identity
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

A _ViewImports.cshtml file can be placed within any folder, in which case it will only be applied to pages or views within that folder and its subfolders.

## Define and render optional and required page sections

### RenderBody
RenderBody is called to render the content of a child view. Any content on said view that is not in a @section will be rendered by RenderBody

```cshtml
@RenderBody()
```

This allows us to separate layout that is common to all views from layout that is specific to a single view.

### RenderSection

RenderSection, following with the "separate content from layout" theme, allows us to designate a place for where content will be rendered that is different from RenderBody()

Inside the _Layout.cshtml for example (by default section are required by each child view):
```cshtml
@RenderSection("footer", required: false)
```

We can designate content to be rendered at RenderSection using a @section declaration.

Inside one of the child view:
```cshtml
@section Footer
{
    <p>Section/Index page</p>
}
```

## Implement partial views and view components for reuse in different areas of the application

### Partial Views

A partial view is a Razor markup file (.cshtml) that renders HTML output within another markup file's rendered output.

Benefits:
 * Break up large markup files into smaller components
 * Reduce the duplication of common markup content across markup files

#### Declare partial views

```cshtlm
@model List<string>
<h1>List of Animals</h1>
<ul>
    @foreach (var item in Model)
    {
        <li>@item</li>
    }
</ul>
```

#### Reference a partial view

Don't use a partial view where complex rendering logic or code execution is required to render the markup. Instead of a partial view, use a view component.

##### Using Partial Tag Helper
```cshtml
<partial name="Shared/_AnimalPartial.cshtml" for="Animals" />
```
A ModelExpression infers the @Model. syntax. For example, for="Animals" can be used instead of for="@Model.Animals".

##### Asynchronous HTML Helper
```cshtml
@await Html.PartialAsync("_AnimalPartial.cshtml", Model.Animals)
```

### ViewComponent

View components are similar to partial views.  View components don't use model binding, and only depend on the data provided when calling into it.

A view component consists of two parts:
 * A class, that is:
   * typically derived from `ViewComponent`
   * or a class with the `[ViewComponent]` attribute, or deriving from a class with the `[ViewComponent]` attribute
   * or a class where the name ends with the suffix `ViewComponent`
 * the result it returns (typically a view). 

A ViewComponent can't use filters, but fully support dependency injection.

#### Create a view component

##### The class:
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewComponentSample.Models;

namespace ViewComponentSample.ViewComponents
{
    public class PriorityListViewComponent : ViewComponent
    {
        private readonly ToDoContext db;

        public PriorityListViewComponent(ToDoContext context)
        {
            db = context;
        }

        // InvokeAsync can take an arbitrary number of arguments
        public async Task<IViewComponentResult> InvokeAsync(int maxPriority, bool isDone)
        {
            var items = await GetItemsAsync(maxPriority, isDone);
            return View(items);
        }

        private Task<List<TodoItem>> GetItemsAsync(int maxPriority, bool isDone)
        {
            return db.ToDo.Where(x => x.IsDone == isDone && x.Priority <= maxPriority).ToListAsync();
        }
    }
}
```

##### The view 
Create the Views/Shared/Components folder. This folder must be named Components.  
Create a Views/Shared/Components/PriorityList/Default.cshtml Razor view.
 * PriorityList folder name must match the name of the view component class, or the name of the class minus the suffix
 * Default is used for the view name by convention

```cshtml
@model IEnumerable<ViewComponentSample.Models.TodoItem>

<h3>Priority Items</h3>
<ul>
    @foreach (var todo in Model)
    {
        <li>@todo.Name</li>
    }
</ul>
```

To specify to an other view (to display a view different than Default.cshtml)
```csharp
public async Task<IViewComponentResult> InvokeAsync(int maxPriority, bool isDone)
{
    // ...
    return View("OtherView", items);
}
```

##### Reference a View Component

Using HTML Helper (c#):
```cshtml
@await Component.InvokeAsync("PriorityList", new { maxPriority = 2, isDone = false })
```

The first argument is the name of the component we want to invoke or call.  
Subsequent parameters are passed to the component. InvokeAsync can take an arbitrary number of arguments.

Using Tag Helper (Razor syntax)
```cshtml
<vc:priority-list max-priority="2" is-done="false">
</vc:priority-list>
```
## Create and use tag and HTML helpers to simplify markup

Tag Helpers enable server-side code to participate in creating and rendering HTML elements in Razor files.

Most built-in Tag Helpers target standard HTML elements and provide server-side attributes for the element

What tag helper provide:
- An HTML-friendly development experience

Interesting Tag Helper:
- asp-controller
- asp-action
- asp-route
- asp-route-{value}

### Create tag helper

Create a C# class:
```csharp
    [HtmlTargetElement("hello-world")]
    public class HelloWorldTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Replace hello-world by span
            output.TagName = "span";
            // Set the content of the HTML tag
            output.Content.SetContent($"HelloWorld @ {DateTime.Now.ToShortTimeString()}");
            // Add an 'time' attribute to the HTML tag
            output.Attributes.SetAttribute("time", DateTime.Now.ToShortTimeString());
            // Specify that the hello-word self closing tag should have start tag and a end tag
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
```

Add the HelloWorldTagHelper to the Views/_ViewImports.cshtml file:
```cshtml
@using HelloWorld
@namespace HelloWorld.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, HelloWorld
```

Use the HelloWorldTagHelper in a Razor page:
```cshtml
<hello-world/>
```
