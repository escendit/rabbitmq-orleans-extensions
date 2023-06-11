// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using System.Diagnostics.CodeAnalysis;
using Configuration;
using Escendit.Orleans.Streaming.RabbitMQ.Options;
using Escendit.Orleans.Streaming.RabbitMQ.Queue;
using Escendit.Orleans.Streaming.RabbitMQ.Stream;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// Silo Builder Extensions.
/// </summary>
[DynamicallyAccessedMembers(
    DynamicallyAccessedMemberTypes.All)]
public static class SiloBuilderExtensions
{
    /// <summary>
    /// Add Rabbit MQ.
    /// </summary>
    /// <param name="builder">The rabbit mq.</param>
    /// <param name="name">The name.</param>
    /// <returns>The client builder.</returns>
    public static RabbitSiloBuilder AddRabbitMqStreaming(
        this ISiloBuilder builder,
        string name)
    {
        return new RabbitSiloBuilder(builder, name);
    }

    /// <summary>
    /// Add Stream System.
    /// </summary>
    /// <param name="siloBuilder">The initial silo builder.</param>
    /// <returns>The updated silo builder.</returns>
    public static RabbitSiloBuilder WithStream(
        this RabbitSiloBuilder siloBuilder)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        return siloBuilder
            .WithStream(_ => { });
    }

    /// <summary>
    /// Add Stream.
    /// </summary>
    /// <param name="siloBuilder">The initial silo builder.</param>
    /// <param name="streamOptions">The options.</param>
    /// <returns>The updated silo builder.</returns>
    public static RabbitSiloBuilder WithStream(
        this RabbitSiloBuilder siloBuilder,
        Action<RabbitStreamOptions> streamOptions)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        ArgumentNullException.ThrowIfNull(streamOptions);
        siloBuilder
            .ConfigureServices(services =>
            {
                var rabbitStreamOptions = new RabbitStreamOptions();

                streamOptions.Invoke(rabbitStreamOptions);

                services
                    .AddOptions<RabbitStreamOptions>(siloBuilder.Name)
                    .Configure(streamOptions);

                services
                    .AddOptions<RabbitOptionsBase>(siloBuilder.Name)
                    .Configure(options =>
                    {
                        options.CacheSize = rabbitStreamOptions.CacheSize;
                        options.ClientProvidedName = rabbitStreamOptions.ClientProvidedName;
                        options.Endpoints.Clear();

                        foreach (var endpoint in rabbitStreamOptions.Endpoints)
                        {
                            options.Endpoints.Add(endpoint);
                        }

                        options.Heartbeat = rabbitStreamOptions.Heartbeat;
                        options.Password = rabbitStreamOptions.Password;
                        options.SslOptions = rabbitStreamOptions.SslOptions;
                        options.StreamFailureHandlerFactory = rabbitStreamOptions.StreamFailureHandlerFactory;
                        options.TotalQueueCount = rabbitStreamOptions.TotalQueueCount;
                        options.UserName = rabbitStreamOptions.UserName;
                        options.VirtualHost = rabbitStreamOptions.VirtualHost;
                    });

                services
                    .ConfigureNamedOptionForLogging<RabbitStreamOptions>(siloBuilder.Name)
                    .ConfigureNamedOptionForLogging<RabbitOptionsBase>(siloBuilder.Name)
                    .TryAddSingleton<DefaultStreamAdapterFactory>();
            });

        _ = new RabbitSiloStreamConfigurator(
            siloBuilder.Name,
            configureServicesDelegate =>
                siloBuilder
                    .ConfigureServices(configureServicesDelegate));

        return siloBuilder;
    }

    /// <summary>
    /// Add Classic Queue System.
    /// </summary>
    /// <param name="siloBuilder">The initial silo builder.</param>
    /// <returns>The updated silo builder.</returns>
    public static RabbitSiloBuilder WithQueue(
        this RabbitSiloBuilder siloBuilder)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        return siloBuilder
            .WithQueue(_ => { });
    }

    /// <summary>
    /// Add Classic Queue System.
    /// </summary>
    /// <param name="siloBuilder">The initial silo builder.</param>
    /// <param name="queueOptions">The queue options.</param>
    /// <returns>The rabbit silo builder.</returns>
    public static RabbitSiloBuilder WithQueue(
        this RabbitSiloBuilder siloBuilder,
        Action<RabbitQueueOptions> queueOptions)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        ArgumentNullException.ThrowIfNull(queueOptions);

        siloBuilder
            .ConfigureServices(services =>
            {
                var rabbitQueueOptions = new RabbitQueueOptions();

                queueOptions.Invoke(rabbitQueueOptions);

                services
                    .AddOptions<RabbitQueueOptions>(siloBuilder.Name)
                    .Configure(queueOptions);

                services
                    .AddOptions<RabbitOptionsBase>(siloBuilder.Name)
                    .Configure(options =>
                    {
                        options.CacheSize = rabbitQueueOptions.CacheSize;
                        options.ClientProvidedName = rabbitQueueOptions.ClientProvidedName;
                        options.Endpoints.Clear();

                        foreach (var endpoint in rabbitQueueOptions.Endpoints)
                        {
                            options.Endpoints.Add(endpoint);
                        }

                        options.Heartbeat = rabbitQueueOptions.Heartbeat;
                        options.Password = rabbitQueueOptions.Password;
                        options.SslOptions = rabbitQueueOptions.SslOptions;
                        options.StreamFailureHandlerFactory = rabbitQueueOptions.StreamFailureHandlerFactory;
                        options.TotalQueueCount = rabbitQueueOptions.TotalQueueCount;
                        options.UserName = rabbitQueueOptions.UserName;
                        options.VirtualHost = rabbitQueueOptions.VirtualHost;
                    });

                services
                    .ConfigureNamedOptionForLogging<RabbitQueueOptions>(siloBuilder.Name)
                    .ConfigureNamedOptionForLogging<RabbitOptionsBase>(siloBuilder.Name)
                    .TryAddSingleton<DefaultQueueAdapterFactory>();
            });

        _ = new RabbitSiloQueueConfigurator(
            siloBuilder.Name,
            configureServicesDelegate =>
                siloBuilder
                    .ConfigureServices(configureServicesDelegate));

        return siloBuilder;
    }
}
