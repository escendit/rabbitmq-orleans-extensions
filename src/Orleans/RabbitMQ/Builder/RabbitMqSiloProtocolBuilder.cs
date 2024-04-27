// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Builder;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Escendit.Orleans.Streaming.RabbitMQ.Tests Silo Protocol Builder.
/// </summary>
public class RabbitMqSiloProtocolBuilder : IRabbitMqSiloProtocolBuilder
{
#if NET8_0_OR_GREATER
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqSiloProtocolBuilder"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="services">The initial services.</param>
    /// <param name="configuration">The configuration.</param>
    public RabbitMqSiloProtocolBuilder(string name, IServiceCollection services, IConfiguration configuration)
    {
        Name = name;
        Services = services;
        Configuration = configuration;
    }
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqSiloProtocolBuilder"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="services">The initial services.</param>
    public RabbitMqSiloProtocolBuilder(string name, IServiceCollection services)
    {
        Name = name;
        Services = services;
    }
#endif

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public IServiceCollection Services { get; }

#if NET8_0_OR_GREATER
    /// <inheritdoc />
    public IConfiguration Configuration { get; }
#endif

    /// <inheritdoc />
    public IServiceCollection Build()
    {
        return Services;
    }
}
