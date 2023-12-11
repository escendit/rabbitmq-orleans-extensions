// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.Configuration;

using Hosting;
using Provider;

/// <summary>
/// Cluster Client Rabbit MQ AMQP Configurator.
/// </summary>
internal class ClusterClientConfigurator : ClusterClientPersistentStreamConfigurator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClusterClientConfigurator"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="clientBuilder">The client builder.</param>
    public ClusterClientConfigurator(
        string name,
        IClientBuilder clientBuilder)
        : base(name, clientBuilder, StreamProtocolAdapterFactory.Create)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(clientBuilder);
        clientBuilder
            .ConfigureServices(configure =>
            {
                configure
                    .AddOrleansNamedSingletonFactory(name, StreamProtocolAdapterFactory.Create)
                    .AddClientStreaming();
            });
    }
}
