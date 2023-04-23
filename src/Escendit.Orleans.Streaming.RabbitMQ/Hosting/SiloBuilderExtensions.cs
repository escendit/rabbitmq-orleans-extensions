// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Configuration;
using Escendit.Orleans.Streaming.RabbitMQ.Options;
using Escendit.Orleans.Streaming.RabbitMQ.Stream;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

/// <summary>
/// Silo Builder Extensions.
/// </summary>
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
    /// Add Stream.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="options">The options.</param>
    /// <returns>The rabbit silo builder.</returns>
    public static RabbitSiloBuilder WithStream(
        this RabbitSiloBuilder builder,
        Action<RabbitStreamOptions>? options = null)
    {
        return builder
            .WithStream(configure => configure.Configure(options ?? new Action<RabbitStreamOptions>(_ => { })));
    }

    /// <summary>
    /// Add Stream.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The rabbit silo builder.</returns>
    public static RabbitSiloBuilder WithStream(
        this RabbitSiloBuilder builder,
        Action<OptionsBuilder<RabbitStreamOptions>>? configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder
            .ConfigureServices(services =>
            {
                services
                    .TryAddSingleton<DefaultStreamAdapterFactory>();
                configureOptions?
                    .Invoke(services.AddOptions<RabbitStreamOptions>(builder.Name));

                services
                    .ConfigureNamedOptionForLogging<RabbitStreamOptions>(builder.Name);
            });

        _ = new RabbitSiloStreamConfigurator(
            builder.Name,
            configureServicesDelegate => builder.ConfigureServices(configureServicesDelegate));

        return builder;
    }
}
