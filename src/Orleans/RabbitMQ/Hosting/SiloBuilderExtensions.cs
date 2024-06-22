// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Hosting;

using Builder;
using global::Orleans.Configuration;
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
#if NET8_0_OR_GREATER
        return new RabbitMqSiloProtocolBuilder(
            name,
            siloBuilder.Services,
            siloBuilder.Configuration);
#else
        return new RabbitMqSiloProtocolBuilder(
            name,
            siloBuilder.Services);
#endif
    }

    /// <summary>
    /// Add Hash Ring Stream Queue Mapper.
    /// </summary>
    /// <param name="siloBuilder">The silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The rabbitmq silo options builder.</returns>
    internal static IRabbitMqSiloOptionsBuilder AddHashRingStreamQueueMapper(
        this IRabbitMqSiloOptionsBuilder siloBuilder,
        Action<HashRingStreamQueueMapperOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        siloBuilder
            .ConfigureServices(services => services
                .AddOrleansNamedSingletonFactory(siloBuilder.Name, ServiceProviderExtensions.CreateDefaultStreamQueueMapper)
                .AddOptions<HashRingStreamQueueMapperOptions>(siloBuilder.Name)
                .Configure(configureOptions));
        return siloBuilder;
    }

    /// <summary>
    /// Add Hash Ring Stream Queue Mapper.
    /// </summary>
    /// <param name="siloBuilder">The silo builder.</param>
    /// <returns>The rabbitmq silo options builder.</returns>
    internal static IRabbitMqSiloOptionsBuilder AddHashRingStreamQueueMapper(
        this IRabbitMqSiloOptionsBuilder siloBuilder)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        return AddHashRingStreamQueueMapper(siloBuilder, _ => { });
    }

    /// <summary>
    /// Add Simple Queue Cache.
    /// </summary>
    /// <param name="siloBuilder">The silo builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The rabbitmq silo options builder.</returns>
    internal static IRabbitMqSiloOptionsBuilder AddSimpleQueueCache(
        this IRabbitMqSiloOptionsBuilder siloBuilder,
        Action<SimpleQueueCacheOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        siloBuilder
            .ConfigureServices(services => services
                .AddOrleansNamedSingletonFactory(siloBuilder.Name, ServiceProviderExtensions.CreateDefaultQueueAdapterCache)
                .AddOptions<SimpleQueueCacheOptions>(siloBuilder.Name)
                .Configure(configureOptions));
        return siloBuilder;
    }

    /// <summary>
    /// Add Simple Queue Cache.
    /// </summary>
    /// <param name="siloBuilder">The silo builder.</param>
    /// <returns>The rabbitmq silo options builder.</returns>
    internal static IRabbitMqSiloOptionsBuilder AddSimpleQueueCache(
        this IRabbitMqSiloOptionsBuilder siloBuilder)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        return AddSimpleQueueCache(siloBuilder, _ => { });
    }
}
