> This chapter should cover:
> - [Read and write string and binary data asynchronously]()
> - [Choose a connection loss strategy]()
> - [Decide when to use Web Sockets]()
> - [Implement SignalR]()
> - [Enable web socket features in an Azure Web App instance]()

## Read and write string and binary data asynchronously

In the Startup class:
```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    // ...some setup code...

    var webSocketOptions = new WebSocketOptions
    {
        KeepAliveInterval = TimeSpan.FromMinutes(2),
        ReceiveBufferSize = 4*1024
    };
    app.UseWebSockets(webSocketOptions);
    app.UseStaticFiles();
    app.UseMVC();
}
```

In a Controller:
```csharp
[HttpGet("{helloWorldId}")]
public async void SayHello(int helloWorldId)
{
    var context = _httpContextAccessor.HttpContext;
    if(context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.Websockets.AcceptWebSocketAsyn();
        
        HelloWorldResult result;
        do
        {
            
            result = _helloWorldService.GetUpdate(helloWorldId);
            Thread.Sleep(2000);

            if(!result.New) continue;

            var jsonMessage = $"{result.Message}";
            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(jsonMessage), 0, result.Message.Lenght), WebSocketMessageType.Text, true, CancellationToken.None); 
        }
        while(!result.Finished);

        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
}
```

Example of code to listen in JavaScript:
```javascript
var exampleSocket = new WebSocket("ws://www.helloworld.com/socketserver", "protocolOne");
exampleSocket.onmessage = function (event) {
  console.log(event.data);
}
```

## Choose a connection loss strategy
## Decide when to use Web Sockets
## Implement SignalR
## Enable web socket features in an Azure Web App instance