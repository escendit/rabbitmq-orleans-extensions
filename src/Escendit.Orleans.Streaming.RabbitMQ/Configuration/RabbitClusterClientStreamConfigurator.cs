// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Escendit.Orleans.Streaming.RabbitMQ.Stream;
using Orleans.Streams;

/// <summary>
/// Cluster Client RabbitMQ Stream Configurator.
/// </summary>
public class RabbitClusterClientStreamConfigurator : ClusterClientPersistentStreamConfigurator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitClusterClientStreamConfigurator"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="clientBuilder">The client builder.</param>
    public RabbitClusterClientStreamConfigurator(
        string name,
        IClientBuilder clientBuilder)
        : base(name, clientBuilder, DefaultStreamAdapterFactory.Create)
    {
    }
}
