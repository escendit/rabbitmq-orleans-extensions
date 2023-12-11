// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Hosting;

using Builder;
using global::Orleans.Configuration;
using global::Orleans.Streams;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class SiloBuilderExtensions
{
    /// <summary>
    /// Add Rabbit MQ on Silo.
    /// </summary>
    /// <param name="siloBuilder">The initial silo builder.</param>
    /// <param name="name">The name.</param>
    /// <returns>The protocol builder.</returns>
    public static IRabbitMqSiloProtocolBuilder AddRabbitMq(
        this ISiloBuilder siloBuilder,
        string name)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        ArgumentNullException.ThrowIfNull(name);
        siloBuilder
            .ConfigureServices(services => services
                .AddOrleansNamedSingletonFactory<Func<QueueId, Task<IStreamFailureHandler>>>(
                    name,
                    (_, _) => ServiceProviderExtensions.HandleFailure));
        return new RabbitMqSiloProtocolBuilder(name, siloBuilder.Services);
    }

    /// <summary>
    /// Add Stream Queue Mapper.
    /// </summary>
    /// <param name="siloBuilder">The silo builder.</param>
    /// <typeparam name="TImplementation">The implementation.</typeparam>
    /// <returns>The rabbitmq silo options builder.</returns>
    public static IRabbitMqSiloOptionsBuilder AddStreamQueueMapper<TImplementation>(
        this IRabbitMqSiloOptionsBuilder siloBuilder)
        where TImplementation : class, IStreamQueueMapper
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        siloBuilder
            .ConfigureServices(services => services
                .AddOrleansNamedSingleton<IStreamQueueMapper, TImplementation>(siloBuilder.Name));
        return siloBuilder;
    }

    /// <summary>
    /// Add Hash Ring Stream Queue Mapper.
    /// </summary>
    /// <param name="siloBuilder">The silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The rabbitmq silo options builder.</returns>
    public static IRabbitMqSiloOptionsBuilder AddHashRingStreamQueueMapper(
        this IRabbitMqSiloOptionsBuilder siloBuilder,
        Action<HashRingStreamQueueMapperOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        siloBuilder
            .ConfigureServices(services => services
                .AddOrleansNamedSingletonFactory(siloBuilder.Name, ServiceProviderExtensions.CreateStreamQueueMapper)
                .AddOptions<HashRingStreamQueueMapperOptions>(siloBuilder.Name)
                .Configure(configureOptions));
        return siloBuilder;
    }

    /// <summary>
    /// Add Queue Adapter Cache.
    /// </summary>
    /// <param name="siloBuilder">The silo builder.</param>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <returns>The rabbitmq silo options builder.</returns>
    public static IRabbitMqSiloOptionsBuilder AddQueueAdapterCache<TImplementation>(
        this IRabbitMqSiloOptionsBuilder siloBuilder)
        where TImplementation : class, IQueueAdapterCache
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        siloBuilder
            .ConfigureServices(services => services
                .AddOrleansNamedSingleton<IQueueAdapterCache, TImplementation>(siloBuilder.Name));
        return siloBuilder;
    }

    /// <summary>
    /// Add Hash Ring Stream Queue Mapper.
    /// </summary>
    /// <param name="siloBuilder">The silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The rabbitmq silo options builder.</returns>
    public static IRabbitMqSiloOptionsBuilder AddSimpleQueueCache(
        this IRabbitMqSiloOptionsBuilder siloBuilder,
        Action<SimpleQueueCacheOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        siloBuilder
            .ConfigureServices(services => services
                .AddOrleansNamedSingletonFactory(siloBuilder.Name, ServiceProviderExtensions.CreateQueueAdapterCache)
                .AddOptions<SimpleQueueCacheOptions>(siloBuilder.Name)
                .Configure(configureOptions));
        return siloBuilder;
    }
}
