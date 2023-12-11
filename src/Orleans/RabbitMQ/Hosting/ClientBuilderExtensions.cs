// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Hosting;

using Builder;
using global::Orleans.Configuration;
using global::Orleans.Providers.Streams.Common;
using global::Orleans.Streams;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrleansCodeGen.Orleans.Concurrency;

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
        clientBuilder
            .ConfigureServices(services => services
                .AddOrleansNamedSingletonFactory<Func<QueueId, Task<IStreamFailureHandler>>>(name, (_, _) => ServiceProviderExtensions.HandleFailure));
        return new RabbitMqClientProtocolBuilder(name, clientBuilder.Services);
    }

    /// <summary>
    /// Add Stream Queue Mapper.
    /// </summary>
    /// <param name="clientBuilder">The client builder.</param>
    /// <typeparam name="TImplementation">The implementation.</typeparam>
    /// <returns>The rabbitmq client options builder.</returns>
    public static IRabbitMqClientOptionsBuilder AddStreamQueueMapper<TImplementation>(
        this IRabbitMqClientOptionsBuilder clientBuilder)
        where TImplementation : class, IStreamQueueMapper
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        clientBuilder
            .ConfigureServices(services => services
                .AddOrleansNamedSingleton<IStreamQueueMapper, TImplementation>(clientBuilder.Name));
        return clientBuilder;
    }

    /// <summary>
    /// Add Hash Ring Stream Queue Mapper.
    /// </summary>
    /// <param name="clientBuilder">The client builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The rabbitmq client options builder.</returns>
    public static IRabbitMqClientOptionsBuilder AddHashRingStreamQueueMapper(
        this IRabbitMqClientOptionsBuilder clientBuilder,
        Action<HashRingStreamQueueMapperOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clientBuilder
            .ConfigureServices(services => services
                .AddOrleansNamedSingletonFactory(clientBuilder.Name, ServiceProviderExtensions.CreateStreamQueueMapper)
                .AddOptions<HashRingStreamQueueMapperOptions>(clientBuilder.Name)
                .Configure(configureOptions));
        return clientBuilder;
    }

    /// <summary>
    /// Add Queue Adapter Cache.
    /// </summary>
    /// <param name="clientBuilder">The client builder.</param>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <returns>The rabbitmq client options builder.</returns>
    public static IRabbitMqClientOptionsBuilder AddQueueAdapterCache<TImplementation>(
        this IRabbitMqClientOptionsBuilder clientBuilder)
        where TImplementation : class, IQueueAdapterCache
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        clientBuilder
            .ConfigureServices(services => services
                .AddOrleansNamedSingleton<IQueueAdapterCache, TImplementation>(clientBuilder.Name));
        return clientBuilder;
    }

    /// <summary>
    /// Add Hash Ring Stream Queue Mapper.
    /// </summary>
    /// <param name="clientBuilder">The client builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The rabbitmq client options builder.</returns>
    public static IRabbitMqClientOptionsBuilder AddSimpleQueueCache(
        this IRabbitMqClientOptionsBuilder clientBuilder,
        Action<SimpleQueueCacheOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        clientBuilder
            .ConfigureServices(services => services
                .AddOrleansNamedSingletonFactory(clientBuilder.Name, ServiceProviderExtensions.CreateQueueAdapterCache)
                .AddOptions<SimpleQueueCacheOptions>(clientBuilder.Name)
                .Configure(configureOptions));
        return clientBuilder;
    }
}
