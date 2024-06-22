// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Builder;

#if NET8_0_OR_GREATER
using Microsoft.Extensions.Configuration;
#endif
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Builder;

/// <summary>
/// Escendit.Orleans.Streaming.RabbitMQ.Tests Silo Options Builder.
/// </summary>
internal class RabbitMqSiloOptionsBuilder : IRabbitMqSiloOptionsBuilder
{
#if NET8_0_OR_GREATER
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqSiloOptionsBuilder"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configurator">The configurator.</param>
    public RabbitMqSiloOptionsBuilder(string name, IServiceCollection services, IConfiguration configuration, SiloPersistentStreamConfigurator configurator)
    {
        Name = name;
        Services = services;
        Configuration = configuration;
        Configurator = configurator;
    }
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqSiloOptionsBuilder"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="services">The services.</param>
    /// <param name="configurator">The configurator.</param>
    public RabbitMqSiloOptionsBuilder(string name, IServiceCollection services, SiloPersistentStreamConfigurator configurator)
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
    public SiloPersistentStreamConfigurator Configurator { get; }

    /// <inheritdoc/>
    public IServiceCollection Build()
    {
        return Services;
    }
}
