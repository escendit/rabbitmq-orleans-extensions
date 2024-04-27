// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Hosting;

using Builder;
using global::Orleans.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class ClientBuilderExtensions
{
    /// <summary>
    /// Add Rabbit MQ on Silo.
    /// </summary>
    /// <param name="clientBuilder">The initial client builder.</param>
    /// <param name="name">The name.</param>
    /// <returns>The protocol builder.</returns>
    public static IRabbitMqClientProtocolBuilder AddRabbitMq(
        this IClientBuilder clientBuilder,
        string name)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        ArgumentNullException.ThrowIfNull(name);
    #if NET8_0_OR_GREATER
        return new RabbitMqClientProtocolBuilder(name, clientBuilder.Services, clientBuilder.Configuration);
    #else
        return new RabbitMqClientProtocolBuilder(name, clientBuilder.Services);
    #endif
    }

    /// <summary>
    /// Add Hash Ring Stream Queue Mapper.
    /// </summary>
    /// <param name="clientBuilder">The client builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The rabbitmq client options builder.</returns>
    internal static IRabbitMqClientOptionsBuilder AddHashRingStreamQueueMapper(
        this IRabbitMqClientOptionsBuilder clientBuilder,
        Action<HashRingStreamQueueMapperOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clientBuilder
            .ConfigureServices(services => services
                .AddOrleansNamedSingletonFactory(clientBuilder.Name, ServiceProviderExtensions.CreateDefaultStreamQueueMapper)
                .AddOptions<HashRingStreamQueueMapperOptions>(clientBuilder.Name)
                .Configure(configureOptions));
        return clientBuilder;
    }

    /// <summary>
    /// Add Hash Ring Stream Queue Mapper.
    /// </summary>
    /// <param name="clientBuilder">The client builder.</param>
    /// <returns>The rabbitmq client options builder.</returns>
    internal static IRabbitMqClientOptionsBuilder AddHashRingStreamQueueMapper(
        this IRabbitMqClientOptionsBuilder clientBuilder)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        return AddHashRingStreamQueueMapper(clientBuilder, _ => { });
    }

    /// <summary>
    /// Add Simple Queue Cache.
    /// </summary>
    /// <param name="clientBuilder">The client builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The rabbitmq client options builder.</returns>
    internal static IRabbitMqClientOptionsBuilder AddSimpleQueueCache(
        this IRabbitMqClientOptionsBuilder clientBuilder,
        Action<SimpleQueueCacheOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clientBuilder
            .ConfigureServices(services => services
                .AddOrleansNamedSingletonFactory(clientBuilder.Name, ServiceProviderExtensions.CreateDefaultQueueAdapterCache)
                .AddOptions<SimpleQueueCacheOptions>(clientBuilder.Name)
                .Configure(configureOptions));
        return clientBuilder;
    }

    /// <summary>
    /// Add Simple Queue Cache.
    /// </summary>
    /// <param name="clientBuilder">The client builder.</param>
    /// <returns>The rabbitmq client options builder.</returns>
    internal static IRabbitMqClientOptionsBuilder AddSimpleQueueCache(
        this IRabbitMqClientOptionsBuilder clientBuilder)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        return AddSimpleQueueCache(clientBuilder, _ => { });
    }
}
