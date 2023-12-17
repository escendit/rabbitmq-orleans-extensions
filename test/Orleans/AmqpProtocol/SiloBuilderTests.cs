// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Tests;

using Collections;
using Fixtures;
using global::Orleans.Streams;
using global::RabbitMQ.Tests.Extensions;
using global::RabbitMQ.Tests.Generators;
using Hosting;
using Xunit.Categories;

/// <summary>
/// Silo Builder Tests.
/// </summary>
[Collection(HostCollectionFixture.Name)]
public class SiloBuilderTests
{
    private readonly HostBuilderFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="SiloBuilderTests"/> class.
    /// </summary>
    /// <param name="fixture">The fixture.</param>
    public SiloBuilderTests(HostBuilderFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <summary>
    /// Add RabbitMQ.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AddRabbitMq()
    {
        var host = _fixture
            .CreateSiloHostBuilder(siloBuilder =>
            {
                siloBuilder
                    .AddRabbitMq("test")
                    .Build();
            })
            .Build();
        Assert.NotNull(host);
    }

    /// <summary>
    /// Add RabbitMQ UseStreamProtocol.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AddRabbitMqUseStreamProtocol()
    {
        var host = _fixture
            .CreateSiloHostBuilder(siloBuilder =>
            {
                siloBuilder
                    .AddRabbitMq("test")
                    .UseAmqpProtocol(ConnectionExtensions.Setup)
                    .Build();
            })
            .Build();
        Assert.NotNull(host);
    }

    /// <summary>
    /// Add RabbitMQ UseStreamProtocol PubSub.
    /// </summary>
    /// <param name="type">The type.</param>
    [Theory]
    [ClassData(typeof(StreamPubSubTypeGenerator))]
    [UnitTest]
    public void AddRabbitMqUseStreamProtocolWithPubSubType(StreamPubSubType type)
    {
        var host = _fixture
            .CreateSiloHostBuilder(siloBuilder =>
            {
                siloBuilder
                    .AddRabbitMq("test")
                    .UseAmqpProtocol(ConnectionExtensions.Setup)
                    .ConfigureStreamPubSub(type)
                    .Build();
            })
            .Build();
        Assert.NotNull(host);
    }
}
