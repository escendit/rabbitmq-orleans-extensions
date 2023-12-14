// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Configuration;

using Hosting;
using Microsoft.Extensions.DependencyInjection;
using Provider;

/// <summary>
/// Silo Configurator.
/// </summary>
internal class SiloConfigurator : SiloPersistentStreamConfigurator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SiloConfigurator"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="configureDelegate">The configure delegate.</param>
    public SiloConfigurator(
        string name,
        Action<Action<IServiceCollection>> configureDelegate)
        : base(name, configureDelegate, AmqpProtocolAdapterFactory.Create)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureDelegate);

        ConfigureDelegate(services =>
        {
            services
                .AddSiloStreaming();
            services
                .AddOrleansNamedSingletonFactory(name, AmqpProtocolAdapterFactory.Create)
                .AddOptions<QueueOptions>(name);
        });
    }
}
