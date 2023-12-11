// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Tests;

using Collections;
using Escendit.Extensions.DependencyInjection.RabbitMQ.Abstractions;
using Escendit.Orleans.Streaming.RabbitMQ.Hosting;
using Fixtures;
using global::Orleans.Streams;
using global::Orleans.TestingHost;
using global::RabbitMQ.Client;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Tests.Grains;

/// <summary>
/// Client Builder Tests.
/// </summary>
[Collection(ClusterCollectionFixture.Name)]
public class ClientBuilderTests
{
    private readonly TestCluster _cluster;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientBuilderTests"/> class.
    /// </summary>
    /// <param name="fixture">The cluster fixture.</param>
    public ClientBuilderTests(ClusterFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _cluster = fixture.Cluster;
    }

    /// <summary>
    /// Start.
    /// </summary>
    [Fact]
    public void Start()
    {
        var connection = _cluster.ServiceProvider.GetOptionalOrleansServiceByName<IConnection>("client");
        Assert.True(connection?.IsOpen);
    }
}
