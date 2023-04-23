// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Orleans.Streaming.RabbitMQ.Queue;

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
    }
}
