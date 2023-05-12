// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Configuration;
using Escendit.Orleans.Streaming.RabbitMQ.Options;
using Escendit.Orleans.Streaming.RabbitMQ.Queue;
using Microsoft.Extensions.DependencyInjection;
using Runtime;

/// <summary>
/// Silo Rabbit MQ Queue Configurator.
/// </summary>
internal class RabbitSiloQueueConfigurator : SiloPersistentStreamConfigurator
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
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureDelegate);

        ConfigureDelegate(services =>
        {
            services
                .AddSingletonNamedService(name, DefaultQueueAdapterFactory.Create)
                .ConfigureNamedOptionForLogging<RabbitQueueOptions>(name);
        });
    }
}
