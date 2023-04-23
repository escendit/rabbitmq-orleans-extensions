// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Orleans.Streaming.RabbitMQ.Options;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Client Builder Extensions.
/// </summary>
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
    /// Add Stream for Rabbit MQ.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The rabbit client builder.</returns>
    public static RabbitClientBuilder WithStream(
        this RabbitClientBuilder builder,
        Action<RabbitStreamOptions> configureOptions)
    {
        return builder;
    }

    /// <summary>
    /// Add Queue for Rabbit MQ.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The rabbit client builder.</returns>
    public static RabbitClientBuilder WithQueue(
        this RabbitClientBuilder builder,
        Action<RabbitQueueOptions> configureOptions)
    {
        builder
            .WithQueue(configure =>
            {
                configure.ConfigureDelegate(services =>
                {
                    services.Configure(configureOptions);
                });
            });

        return builder;
    }

    /// <summary>
    /// Add Rabbit MQ Streams.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>The client builder.</returns>
    public static RabbitClientBuilder WithQueue(
        this RabbitClientBuilder builder,
        Action<RabbitClusterClientStreamConfigurator>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var configurator = new RabbitClusterClientStreamConfigurator(builder.Name, builder);
        configure?.Invoke(configurator);
        return builder;
    }
}
