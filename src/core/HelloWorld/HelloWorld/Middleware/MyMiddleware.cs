using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloWorld.Middleware
{
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
}
