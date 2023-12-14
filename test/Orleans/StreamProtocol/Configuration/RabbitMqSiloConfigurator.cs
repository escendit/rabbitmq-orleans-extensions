// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.Tests.Configuration;

using Extensions.DependencyInjection.RabbitMQ.Abstractions;
using global::Orleans.TestingHost;
using Hosting;

/// <summary>
/// Rabbit MQ Silo Configurator.
/// </summary>
public class RabbitMqSiloConfigurator : ISiloConfigurator
{
    /// <inheritdoc />
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder
            .AddMemoryGrainStorageAsDefault()
            .AddMemoryGrainStorage("PubSubStore")
            .AddStreaming()
            .AddRabbitMq("silo")
            .UseStreamProtocol(options =>
            {
                options.UserName = "guest";
                options.Password = "guest";
                options.VirtualHost = "/";
                options.Endpoints.Add(new Endpoint
                {
                    HostName = "localhost",
                    Port = 5552,
                });
            })
            .AddHashRingStreamQueueMapper()
            .AddSimpleQueueCache()
            .Build();
    }
}
