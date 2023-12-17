// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.Builder;

using Escendit.Orleans.Streaming.RabbitMQ.Builder;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Escendit.Orleans.Streaming.RabbitMQ.Tests Client Options Builder.
/// </summary>
internal sealed class RabbitMqClientOptionsBuilder : IRabbitMqClientOptionsBuilder
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

    /// <inheritdoc/>
    public ClusterClientPersistentStreamConfigurator Configurator { get; }

    /// <inheritdoc/>
    public IServiceCollection Build()
    {
        return Services;
    }
}
