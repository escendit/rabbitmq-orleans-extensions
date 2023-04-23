// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Tests.Configuration;

using global::Orleans.TestingHost;
using Options;

/// <summary>
/// Rabbit MQ Silo Configurator.
/// </summary>
public class RabbitMQSiloConfigurator : ISiloConfigurator
{
    /// <inheritdoc />
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder
            .AddStreaming()
            .AddMemoryGrainStorageAsDefault()
            .AddRabbitMqStreaming("Default")
            .WithStream(options =>
            {
                options.Endpoints.Add(new RabbitEndpoint { HostName = "localhost", Port = 5552 });
                options.UserName = "test";
                options.Password = "test";
                options.VirtualHost = "testing";
            });
    }
}
