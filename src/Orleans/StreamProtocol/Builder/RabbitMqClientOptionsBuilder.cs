// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.Builder;

using Escendit.Orleans.Streaming.RabbitMQ.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Escendit.Orleans.Streaming.RabbitMQ.Tests Client Options Builder.
/// </summary>
internal sealed class RabbitMqClientOptionsBuilder : IRabbitMqClientOptionsBuilder
{
#if NET8_0_OR_GREATER
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqClientOptionsBuilder"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configurator">The configurator.</param>
    public RabbitMqClientOptionsBuilder(
        string name,
        IServiceCollection services,
        IConfiguration configuration,
        ClusterClientPersistentStreamConfigurator configurator)
    {
        Name = name;
        Services = services;
        Configuration = configuration;
        Configurator = configurator;
    }
#else
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
#endif

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
