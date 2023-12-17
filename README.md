# RabbitMQ Orleans Extensions

Use RabbitMQ Streaming Provider for [Orleans](https://github.com/dotnet/orleans) in two flavors
- [AMQP Protocol](src/Orleans/AmqpProtocol)
- [Stream Protocol](src/Orleans/StreamProtocol)

## Installation

### AMQP Protocol

To install Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol, run the following command in the Package Manager Console:

```powershell
Install-Package Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol
```

### Stream Protocol

To install Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol, run the following command in the Package Manager Console:

```powershell
Install-Package Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol
```

## Usage

### AMQP Protocol
To use Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol first register the stream provider
using the `AddRabbitMq` with `UseAmqpProtocol` method in the Orleans configuration:

#### SiloBuilder

```csharp
var host = Host
    .CreateDefaultBuilder()
    .UseOrleans(siloBuilder => siloBuilder
        .AddRabbitMq("ProviderName")
        .UseAmqpProtocol(...)
        .Build())
    .Build();
```

#### ClientBuilder

```csharp
var host = Host
    .CreateDefaultBuilder()
    .UseOrleansClient(clientBuilder => clientBuilder
        .AddRabbitMq("ProviderName")
        .UseAmqpProtocol(...)
        .Build())
    .Build();
```

### Stream Protocol

To use Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol register the stream provider
using the `AddRabbitMq` with `UseStreamProtocol` method in the Orleans configuration:

#### SiloBuilder

```csharp
var host = Host
    .CreateDefaultBuilder()
    .UseOrleans(siloBuilder => siloBuilder
        .AddRabbitMq("ProviderName")
        .UseStreamProtocol(...)
        .Build())
    .Build();
```

#### ClientBuilder

```csharp
var host = Host
    .CreateDefaultBuilder()
    .UseOrleansClient(clientBuilder => clientBuilder
        .AddRabbitMq("ProviderName")
        .UseStreamProtocol(...)
        .Build())
    .Build();
```

## Contributing

If you'd like to contribute to rabbitmq-orleans-extensions,
please fork the repository and make changes as you'd like.
Pull requests are warmly welcome.
