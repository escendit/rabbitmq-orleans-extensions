// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Orleans.Streaming.RabbitMQ.Options;
using Escendit.Orleans.Streaming.RabbitMQ.Stream;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Runtime;

/// <summary>
/// Silo Rabbit MQ Stream Configurator.
/// </summary>
public class RabbitSiloStreamConfigurator : SiloPersistentStreamConfigurator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitSiloStreamConfigurator"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="configureDelegate">The configure delegate.</param>
    public RabbitSiloStreamConfigurator(
        string name,
        Action<Action<IServiceCollection>> configureDelegate)
        : base(name, configureDelegate, DefaultStreamAdapterFactory.Create)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureDelegate);

        ConfigureDelegate(services =>
        {
            services
                .AddSingletonNamedService(name, DefaultStreamAdapterFactory.Create)
                .ConfigureNamedOptionForLogging<RabbitStreamOptions>(name);
        });
    }
}
