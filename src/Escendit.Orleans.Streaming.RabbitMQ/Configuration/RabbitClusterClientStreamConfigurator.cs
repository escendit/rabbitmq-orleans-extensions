// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Configuration;
using Escendit.Orleans.Streaming.RabbitMQ.Options;
using Escendit.Orleans.Streaming.RabbitMQ.Stream;
using Runtime;

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
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(clientBuilder);

        clientBuilder
            .Services
            .AddClientStreaming();

        clientBuilder
            .ConfigureServices(configure =>
            {
                configure
                    .AddSingletonNamedService(name, DefaultStreamAdapterFactory.Create)
                    .ConfigureNamedOptionForLogging<RabbitStreamOptions>(name);
            });
    }
}
