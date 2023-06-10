// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Hosting;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Rabbit Connection Factory Builder.
/// </summary>
public interface IRabbitConnectionFactoryBuilder
{
    /// <summary>
    /// Gets the service collection.
    /// </summary>
    /// <value>The services.</value>
    IServiceCollection Services { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    string Name { get; }
}
