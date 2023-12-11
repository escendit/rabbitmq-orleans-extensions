﻿// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Builder;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Escendit.Orleans.Streaming.RabbitMQ.Tests Client Protocol Builder.
/// </summary>
public class RabbitMqClientProtocolBuilder : IRabbitMqClientProtocolBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqClientProtocolBuilder"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="services">The services.</param>
    public RabbitMqClientProtocolBuilder(string name, IServiceCollection services)
    {
        Name = name;
        Services = services;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public IServiceCollection Services { get; }

    /// <inheritdoc />
    public IServiceCollection Build()
    {
        return Services;
    }
}
