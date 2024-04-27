// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Builder;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Builder;

/// <summary>
/// Escendit.Orleans.Streaming.RabbitMQ.Tests Client Options Builder.
/// </summary>
internal class RabbitMqClientOptionsBuilder : IRabbitMqClientOptionsBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqClientOptionsBuilder"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="services">The services.</param>
    /// <param name="configurator">The configurator.</param>
    public RabbitMqClientOptionsBuilder(
        string name,
        IServiceCollection services,
        ClusterClientPersistentStreamConfigurator configurator)
    {
        Name = name;
        Services = services;
        Configurator = configurator;
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }

#if NET8_0_OR_GREATER
    /// <inheritdoc/>
    public IConfiguration Configuration { get; }
#endif

    /// <inheritdoc/>
    public ClusterClientPersistentStreamConfigurator Configurator { get; }

    /// <inheritdoc/>
    public IServiceCollection Build()
    {
        return Services;
    }
}
