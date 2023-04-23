// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Orleans.Streaming.RabbitMQ.Queue;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Streams;

/// <summary>
/// Silo Rabbit MQ Queue Configurator.
/// </summary>
public class RabbitSiloQueueConfigurator : SiloPersistentStreamConfigurator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitSiloQueueConfigurator"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="configureDelegate">The configure delegate.</param>
    public RabbitSiloQueueConfigurator(
        string name,
        Action<Action<IServiceCollection>> configureDelegate)
        : base(name, configureDelegate, DefaultQueueAdapterFactory.Create)
    {
    }
}
