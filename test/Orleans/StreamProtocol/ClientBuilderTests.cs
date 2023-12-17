// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.Tests;

using Collections;
using Fixtures;
using global::Orleans.Streams;
using global::RabbitMQ.Tests.Extensions;
using global::RabbitMQ.Tests.Generators;
using Hosting;
using Microsoft.Extensions.Hosting;
using Xunit.Categories;

/// <summary>
/// Client Builder Tests.
/// </summary>
[Collection(HostCollectionFixture.Name)]
public class ClientBuilderTests
{
    private readonly HostBuilderFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientBuilderTests"/> class.
    /// </summary>
    /// <param name="fixture">The fixture.</param>
    public ClientBuilderTests(HostBuilderFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <summary>
    /// Add Rabbit MQ.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AddRabbitMq()
    {
        var host = _fixture
            .CreateClusterClientHostBuilder(clientBuilder =>
            {
                clientBuilder
                    .AddRabbitMq("test");
            })
            .Build();
        Assert.NotNull(host);
    }

    /// <summary>
    /// Add RabbitMQ Use Stream Protocol.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AddRabbitMqUseStreamProtocol()
    {
        var host = _fixture
            .CreateClusterClientHostBuilder(clientBuilder =>
            {
                clientBuilder
                    .AddRabbitMq("test")
                    .UseStreamProtocol(ConnectionExtensions.Setup)
                    .Build();
            });
        Assert.NotNull(host);
    }

    /// <summary>
    /// Add RabbitMQ UseStreamProtocol UsePubSubType.
    /// </summary>
    /// <param name="type"> The type.</param>
    [Theory]
    [ClassData(typeof(StreamPubSubTypeGenerator))]
    [UnitTest]
    public void AddRabbitMqUseStreamProtocolUsePubSubType(StreamPubSubType type)
    {
        var host = _fixture
            .CreateClusterClientHostBuilder(clientBuilder =>
            {
                clientBuilder
                    .AddRabbitMq("test")
                    .UseStreamProtocol(ConnectionExtensions.Setup)
                    .ConfigureStreamPubSub(type)
                    .Build();
            })
            .Build();
        Assert.NotNull(host);
    }
}
