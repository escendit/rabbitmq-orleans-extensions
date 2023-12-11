// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Extensions.DependencyInjection.RabbitMQ.Abstractions;
using Escendit.Orleans.Streaming.RabbitMQ.Builder;
using Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.Builder;
using Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.Configuration;

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
    public static IRabbitMqSiloOptionsBuilder UseStreamProtocol(this IRabbitMqSiloProtocolBuilder siloBuilder, Action<ConnectionOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        ArgumentNullException.ThrowIfNull(configureOptions);
        return new RabbitMqSiloOptionsBuilder(
            siloBuilder.Name,
            siloBuilder.Services,
            new SiloConfigurator(
            siloBuilder.Name,
            configureDelegate => siloBuilder
                .ConfigureServices(configureDelegate)));
    }
}
