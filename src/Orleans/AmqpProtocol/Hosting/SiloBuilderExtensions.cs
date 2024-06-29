// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Extensions.DependencyInjection.RabbitMQ.Abstractions;
using Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Builder;
using Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Configuration;
using Escendit.Orleans.Streaming.RabbitMQ.Builder;
using Escendit.Orleans.Streaming.RabbitMQ.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Streams;

/// <summary>
/// Silo Builder Extensions.
/// </summary>
public static class SiloBuilderExtensions
{
    /// <summary>
    /// Use AMQP Protocol.
    /// </summary>
    /// <param name="siloBuilder">The silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The client options builder.</returns>
    public static IRabbitMqSiloOptionsBuilder UseAmqpProtocol(this IRabbitMqSiloProtocolBuilder siloBuilder, Action<ConnectionOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        return new RabbitMqSiloOptionsBuilder(
            siloBuilder.Name,
            siloBuilder.Services,
#if NET8_0_OR_GREATER
            siloBuilder.Configuration,
#endif
            new SiloConfigurator(
            siloBuilder.Name,
            configureDelegate => siloBuilder
                .ConfigureServices(configureDelegate)
                .ConfigureServices(services => services
                    .AddRabbitMqConnection(siloBuilder.Name, configureOptions))))
            .AddHashRingStreamQueueMapper()
            .AddSimpleQueueCache();
    }

    /// <summary>
    /// Configure Stream Pub Sub.
    /// </summary>
    /// <param name="siloBuilder">The initial silo options builder.</param>
    /// <param name="streamPubSubType">The stream pub sub type.</param>
    /// <returns>The rabbitmq client options builder.</returns>
    public static IRabbitMqSiloOptionsBuilder ConfigureStreamPubSub(this IRabbitMqSiloOptionsBuilder siloBuilder, StreamPubSubType streamPubSubType = StreamPubSubType.ExplicitGrainBasedAndImplicit)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        siloBuilder.Configurator.ConfigureStreamPubSub(streamPubSubType);
        return siloBuilder;
    }

    /// <summary>
    /// Configure Default Stream Failure Handler.
    /// </summary>
    /// <param name="siloOptionsBuilder">The initial silo options builder.</param>
    /// <returns>The updated silo options builder.</returns>
    public static IRabbitMqSiloOptionsBuilder ConfigureDefaultStreamDeliveryFailureHandler(this IRabbitMqSiloOptionsBuilder siloOptionsBuilder)
    {
        siloOptionsBuilder.ConfigureServices(services =>
        {
            services.ConfigureOptions<ConfigureQueueOptions>();
        });
        return siloOptionsBuilder;
    }
}
