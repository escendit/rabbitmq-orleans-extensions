// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Microsoft.Extensions.DependencyInjection;

using Escendit.Orleans.Streaming.RabbitMQ.Hosting;

/// <summary>
/// Rabbit Connection Factory Builder.
/// </summary>
public class RabbitConnectionFactoryBuilder : IRabbitConnectionFactoryBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitConnectionFactoryBuilder"/> class.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="name">The name.</param>
    internal RabbitConnectionFactoryBuilder(IServiceCollection services, string name)
    {
        Services = services;
        Name = name;
    }

    /// <inheritdoc />
    public IServiceCollection Services { get; }

    /// <inheritdoc />
    public string Name { get; }
}
