// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Builder;
using Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Configuration;
using Escendit.Orleans.Streaming.RabbitMQ.Builder;
using Escendit.Orleans.Streaming.RabbitMQ.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Streams;
using ConnectionOptions = Escendit.Extensions.DependencyInjection.RabbitMQ.Abstractions.ConnectionOptions;

/// <summary>
/// Client Builder Extensions.
/// </summary>
public static class ClientBuilderExtensions
{
    /// <summary>
    /// Use AMQP Protocol.
    /// </summary>
    /// <param name="clientBuilder">The client builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The client options builder.</returns>
    public static IRabbitMqClientOptionsBuilder UseAmqpProtocol(this IRabbitMqClientProtocolBuilder clientBuilder, Action<ConnectionOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        var configurator = new ClusterClientConfigurator(clientBuilder.Name, clientBuilder);
        configurator.Configure<ConnectionOptions>(options => options.Configure(configureOptions));
        clientBuilder.ConfigureServices(services => services
            .AddRabbitMqConnection(clientBuilder.Name, configureOptions));
        return new RabbitMqClientOptionsBuilder(
                clientBuilder.Name,
                clientBuilder.Services,
#if NET8_0_OR_GREATER
                clientBuilder.Configuration,
#endif
                configurator)
            .AddSimpleQueueCache()
            .AddHashRingStreamQueueMapper();
    }

    /// <summary>
    /// Configure Stream Pub Sub.
    /// </summary>
    /// <param name="clientBuilder">The initial client options builder.</param>
    /// <param name="streamPubSubType">The stream pub sub type.</param>
    /// <returns>The rabbitmq client options builder.</returns>
    public static IRabbitMqClientOptionsBuilder ConfigureStreamPubSub(this IRabbitMqClientOptionsBuilder clientBuilder, StreamPubSubType streamPubSubType = StreamPubSubType.ExplicitGrainBasedAndImplicit)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        clientBuilder.Configurator.ConfigureStreamPubSub(streamPubSubType);
        return clientBuilder;
    }
}
