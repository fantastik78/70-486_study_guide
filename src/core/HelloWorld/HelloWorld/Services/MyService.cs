using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloWorld.Services
{
    public class MyService : IMyService
    {
        public string SayHelloWorld()
        {
            return "Hello world from service";
        }
    }
}
