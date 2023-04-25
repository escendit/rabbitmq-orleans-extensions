// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Tests.Configuration;

using global::Orleans.TestingHost;
using Options;

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
            /*.AddRabbitMqStreaming("Stream")
            .WithStream(options =>
            {
                options.Endpoints.Add(new RabbitEndpoint { HostName = "localhost", Port = 5552 });
                options.UserName = "test";
                options.Password = "test";
                options.VirtualHost = "testing";
                options.ClientProvidedName = "Silo-Stream";
            })*/
            .AddRabbitMqStreaming("Queue")
            .WithQueue(options =>
            {
                options.Endpoints.Add(new RabbitEndpoint { HostName = "localhost", Port = 5672 });
                options.UserName = "test";
                options.Password = "test";
                options.VirtualHost = "testing";
                options.ClientProvidedName = "Silo-Queue";
            });
    }
}
