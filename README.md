# NuGet Package: Escendit.Orleans.Streaming.RabbitMQ

Escendit.Orleans.Streaming.RabbitMQ is a NuGet Package
that integrates RabbitMQ with [Orleans](https://github.com/dotnet/orleans) Streaming Provider.

The Library contains 2 different ways of streaming, first via Stream Protocol, and other via AMQP protocol.

## Installation

To install Escendit.Orleans.Streaming.RabbitMQ, run the following command in the Package Manager Console:

```powershell
Install-Package Escendit.Orleans.Streaming.RabbitMQ
```

## Usage

To use Escendit.Orleans.Streaming.RabbitMQ first register the stream provider
using the `AddRabbitMqStreaming` method in the Orleans configuration:

#### SiloBuilder

```csharp
var builder = new SiloBuilder()
    .AddRabbitMqStreaming("ProviderName");
```

#### ClientBuilder

```csharp
var builder = new ClientBuilder()
    .AddRabbitMqStreaming("ProviderName");
```

### Stream Protocol

To use Escendit.Orleans.Streaming.RabbitMQ with Stream Protocol,
first register the stream provider using the `AddRabbitMqStreaming` method in the Orleans configuration,
and then call the `WithStream` method to specify that the provider should use the Stream Protocol:

#### SiloBuilder

```csharp
var hostBuilder = Host
    .CreateDefaultBuilder()
    .UseOrleans(builder =>
    {
        builder
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "cluster-id";
                options.ServiceId = "service-id";
            })
            .AddStreaming()
            .AddRabbitMqStreaming("ProviderName")
            .WithStream(options =>
            {
                options.Endpoints.Add(new RabbitEndpoint { HostName = "localhost", Port = 5552 });
                options.UserName = "guest";
                options.Password = "guest";
                options.VirtualHost = "/";
            });
    });
```

#### ClientBuilder

```csharp
var hostBuilder = Host
    .CreateDefaultBuilder()
    .UseOrleansClient(builder =>
    {
        builder
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "cluster-id";
                options.ServiceId = "service-id";
            })
            .AddStreaming()
            .AddRabbitMqStreaming("ProviderName")
            .WithStream(options =>
            {
                options.Endpoints.Add(new RabbitEndpoint { HostName = "localhost", Port = 5552 });
                options.UserName = "guest";
                options.Password = "guest";
                options.VirtualHost = "/";
            });            
    })
```

### AMQP Protocol

To use Escendit.Orleans.Streaming.RabbitMQ with Stream Protocol,
first register the stream provider using the `AddRabbitMqStreaming` method in the Orleans configuration,
and then call the `WithQueue` method to specify that the provider should use the AMQP Protocol:

#### SiloBuilder

```csharp
var hostBuilder = Host
    .CreateDefaultBuilder()
    .UseOrleans(builder =>
    {
        builder
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "cluster-id";
                options.ServiceId = "service-id";
            })
            .AddStreaming()
            .AddRabbitMqStreaming("ProviderName")
            .WithQueue(options =>
            {
                options.Endpoints.Add(new RabbitEndpoint { HostName = "localhost", Port = 5672 });
                options.UserName = "guest";
                options.Password = "guest";
                options.VirtualHost = "/";
            });
    });
```

#### ClientBuilder

```csharp
var hostBuilder = Host
    .CreateDefaultBuilder()
    .UseOrleansClient(builder =>
    {
        builder
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "cluster-id";
                options.ServiceId = "service-id";
            })
            .AddStreaming()
            .AddRabbitMqStreaming("ProviderName")
            .WithQueue(options =>
            {
                options.Endpoints.Add(new RabbitEndpoint { HostName = "localhost", Port = 5672 });
                options.UserName = "guest";
                options.Password = "guest";
                options.VirtualHost = "/";
            });            
    })
```

## Consume

Create a stream:

```csharp
public class MyGrain : Grain, IMyGrain
{
    private IAsyncStream<T> _stream;
    public override async Task OnActivateAsync()
    {
        var streamProvider = this.GetStreamProvider("ProviderName");
        _stream = _streamProvider.GetStream<int>(this.GetPrimaryKey(), "stream-namespace");
        _stream.SubscribeAsync(ReceiveMessage);
        await base.OnActivateAsync();
    }

    public async Task SendMessage()
    {
        // Send a message
        await _stream.OnNextAsync(42);
    }
    
    public Task ReceiveMessage(Event @event)
    {
        // Receive a message.
    }
}
```

## Contributing

If you'd like to contribute to Escendit.Orleans.Streaming.RabbitMQ,
please fork the repository and make changes as you'd like.
Pull requests are warmly welcome.
