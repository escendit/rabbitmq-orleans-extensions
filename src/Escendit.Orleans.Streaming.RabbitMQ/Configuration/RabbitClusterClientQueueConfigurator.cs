// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Configuration;
using Escendit.Orleans.Streaming.RabbitMQ.Options;
using Escendit.Orleans.Streaming.RabbitMQ.Queue;
using Runtime;

/// <summary>
/// Cluster Client Rabbit MQ Queue Configurator.
/// </summary>
public class RabbitClusterClientQueueConfigurator : ClusterClientPersistentStreamConfigurator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitClusterClientQueueConfigurator"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="clientBuilder">The client builder.</param>
    public RabbitClusterClientQueueConfigurator(
        string name,
        IClientBuilder clientBuilder)
        : base(name, clientBuilder, DefaultQueueAdapterFactory.Create)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(clientBuilder);

        clientBuilder
            .Services
            .AddClientStreaming();

        clientBuilder
            .ConfigureServices(configure =>
            {
                configure
                    .AddSingletonNamedService(name, DefaultQueueAdapterFactory.Create)
                    .ConfigureNamedOptionForLogging<RabbitQueueOptions>(name);
            });
    }
}
