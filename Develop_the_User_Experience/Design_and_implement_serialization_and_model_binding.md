> This chapter should cover:
> - [Serialize models and data using supported serialization formats, including JSON, XML, protobuf, and WCF/SOAP](#serialize-models-and-data-using-supported-serialization-formats-including-json-xml-protobuf-and-wcfsoap)
> - Implement model and property binding, including custom binding and model validation
> - Implement web socket communication in MVC
> - Implement file uploading and multipart data
> - Use AutoRest to build clients

##### Preparation resources
> * FormCollection Class: http://msdn.microsoft.com/en-us/library/system.web.mvc.formcollection(v=vs.118).aspx

## Serialize models and data using supported serialization formats, including JSON, XML, protobuf, and WCF/SOAP
## Implement model and property binding, including custom binding and model validation

IValidatableObject allow to write custom code to perform validation checks.

Controller method bindings:
* [FromBody]
* [FromForm]
* [FromQuery]
* [FromRoute]
* [FromHeader]
* [FromServices]

FromServices attribute to inject the dependency
```csharp
public class HelloWorldController : Controller
{  
    [HttpGet]
    public ActionResult<bool> Get([FromServices] IHelloWorldService service, int helloWorldId)
    {
        return service.Find(helloWorldId);
    }
}
```

## Implement web socket communication in MVC
## Implement file uploading and multipart data
## Use AutoRest to build clients