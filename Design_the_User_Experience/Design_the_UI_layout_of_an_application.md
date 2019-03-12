> This chapter should cover:
> - [Design and implement pages by using Razor Pages]()
> - [Design and implement layouts to provide visual structure]()
> - [Define and render optional and required page sections]()
> - [Implement partial views and view components for reuse in different areas of the application]()
> - [Create and use tag and HTML helpers to simplify markup]()

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

RenderBody
RenderSection

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

```csharp

```