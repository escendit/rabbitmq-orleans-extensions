# RabbitMQ Orleans AMQP Protocol Streaming Provider

Use RabbitMQ Streaming Provider for [Orleans](https://github.com/dotnet/orleans) with AMQP Protocol

## Installation

To install Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol, run the following command in the Package Manager Console:

```powershell
Install-Package Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol
```

## Usage

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

## Contributing

If you'd like to contribute to rabbitmq-orleans-extensions,
please fork the repository and make changes as you'd like.
Pull requests are warmly welcome.
