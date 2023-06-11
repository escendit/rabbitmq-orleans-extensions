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
/// Client Builder Extensions.
/// </summary>
[DynamicallyAccessedMembers(
    DynamicallyAccessedMemberTypes.All)]
public static class ClientBuilderExtensions
{
    /// <summary>
    /// Add Rabbit MQ.
    /// </summary>
    /// <param name="builder">The rabbit mq.</param>
    /// <param name="name">The name.</param>
    /// <returns>The client builder.</returns>
    public static RabbitClientBuilder AddRabbitMqStreaming(
        this IClientBuilder builder,
        string name)
    {
        return new RabbitClientBuilder(builder, name);
    }

    /// <summary>
    /// Add Stream System.
    /// </summary>
    /// <param name="clientBuilder">The initial client builder.</param>
    /// <returns>The updated client builder.</returns>
    public static RabbitClientBuilder WithStream(
        this RabbitClientBuilder clientBuilder)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        return clientBuilder
            .WithStream(_ => { });
    }

    /// <summary>
    /// Add Stream for Rabbit MQ.
    /// </summary>
    /// <param name="clientBuilder">The builder.</param>
    /// <param name="streamOptions">The configure options.</param>
    /// <returns>The rabbit client builder.</returns>
    public static RabbitClientBuilder WithStream(
        this RabbitClientBuilder clientBuilder,
        Action<RabbitStreamOptions> streamOptions)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);

        clientBuilder
            .ConfigureServices(services =>
            {
                var rabbitStreamOptions = new RabbitStreamOptions();

                streamOptions.Invoke(rabbitStreamOptions);

                services
                    .AddOptions<RabbitStreamOptions>(clientBuilder.Name)
                    .Configure(streamOptions);

                services
                    .AddOptions<RabbitOptionsBase>(clientBuilder.Name)
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
                    .ConfigureNamedOptionForLogging<RabbitStreamOptions>(clientBuilder.Name)
                    .ConfigureNamedOptionForLogging<RabbitOptionsBase>(clientBuilder.Name)
                    .TryAddSingleton<DefaultStreamAdapterFactory>();
            });

        _ = new RabbitClusterClientStreamConfigurator(
            clientBuilder.Name,
            clientBuilder);

        return clientBuilder;
    }

    /// <summary>
    /// Add Classic Queue System.
    /// </summary>
    /// <param name="clientBuilder">The initial client builder.</param>
    /// <returns>The updated client builder.</returns>
    public static RabbitClientBuilder WithQueue(
        this RabbitClientBuilder clientBuilder)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        return clientBuilder
            .WithQueue(_ => { });
    }

    /// <summary>
    /// Add Queue for Rabbit MQ.
    /// </summary>
    /// <param name="clientBuilder">The builder.</param>
    /// <param name="queueOptions">The options.</param>
    /// <returns>The rabbit client builder.</returns>
    public static RabbitClientBuilder WithQueue(
        this RabbitClientBuilder clientBuilder,
        Action<RabbitQueueOptions> queueOptions)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);

        clientBuilder
            .ConfigureServices(services =>
            {
                var rabbitQueueOptions = new RabbitQueueOptions();

                queueOptions.Invoke(rabbitQueueOptions);

                services
                    .AddOptions<RabbitQueueOptions>(clientBuilder.Name)
                    .Configure(queueOptions);

                services
                    .AddOptions<RabbitOptionsBase>(clientBuilder.Name)
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
                    .ConfigureNamedOptionForLogging<RabbitQueueOptions>(clientBuilder.Name)
                    .ConfigureNamedOptionForLogging<RabbitOptionsBase>(clientBuilder.Name)
                    .TryAddSingleton<DefaultQueueAdapter>();
            });

        _ = new RabbitClusterClientQueueConfigurator(
            clientBuilder.Name,
            clientBuilder);

        return clientBuilder;
    }
}
