﻿// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Tests.Configuration;

using Escendit.Extensions.DependencyInjection.RabbitMQ.Abstractions;
using Escendit.Orleans.Streaming.RabbitMQ.Hosting;
using global::Orleans.TestingHost;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Test Client Configurator.
/// </summary>
public class RabbitMqClientConfigurator : IClientBuilderConfigurator
{
    /// <inheritdoc />
    public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
    {
        clientBuilder
            .AddRabbitMq("client")
            .UseAmqpProtocol(options =>
            {
                options.UserName = "guest";
                options.Password = "guest";
                options.VirtualHost = "/";
                options.Endpoints.Add(new Endpoint
                {
                    HostName = "localhost",
                    Port = 5672,
                });
            })
            .AddHashRingStreamQueueMapper(options => { })
            .AddSimpleQueueCache(_ => { })
            .Build();
    }
}
