// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Tests.Configuration;

using global::Orleans.TestingHost;
using Microsoft.Extensions.Configuration;
using Options;

/// <summary>
/// Test Client Configurator.
/// </summary>
public class RabbitMqClientConfigurator : IClientBuilderConfigurator
{
    /// <inheritdoc />
    public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
    {
        clientBuilder
            .AddStreaming()
            /*.AddRabbitMqStreaming("Stream")
            .WithStream(options =>
            {
                options.Endpoints.Add(new RabbitEndpoint { HostName = "localhost", Port = 5552 });
                options.UserName = "test";
                options.Password = "test";
                options.VirtualHost = "testing";
                options.ClientProvidedName = "Client-Stream";
            })*/
            .AddRabbitMqStreaming("Queue")
            .WithQueue(options =>
            {
                options.Endpoints.Add(new RabbitEndpoint { HostName = "localhost", Port = 5672 });
                options.UserName = "test";
                options.Password = "test";
                options.VirtualHost = "testing";
                options.ClientProvidedName = "Client-Queue";
            });
    }
}
