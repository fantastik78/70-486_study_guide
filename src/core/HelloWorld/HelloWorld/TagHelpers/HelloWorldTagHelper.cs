using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HelloWorld.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
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
}
