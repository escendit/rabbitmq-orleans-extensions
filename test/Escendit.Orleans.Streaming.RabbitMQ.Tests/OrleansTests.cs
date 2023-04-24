// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Tests;

using System.ComponentModel;
using Escendit.Orleans.Streaming.RabbitMQ.Tests.Collections;
using Escendit.Orleans.Streaming.RabbitMQ.Tests.Fixtures;
using Escendit.Orleans.Streaming.RabbitMQ.Tests.Grains;
using global::Orleans.TestingHost;
using Xunit.Categories;

/// <summary>
/// Orleans Tests.
/// </summary>
[Collection(ClusterCollectionFixture.Name)]
public class OrleansTests
{
    private readonly TestCluster _cluster;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrleansTests"/> class.
    /// </summary>
    /// <param name="fixture">The fixture.</param>
    public OrleansTests(ClusterFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _cluster = fixture.Cluster;
    }

    /// <summary>
    /// Test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [UnitTest]
    public async Task TestActionAsync()
    {
        // Method intentionally left empty.
        var producerService = _cluster.GrainFactory.GetGrain<IProducerService>(Guid.Empty);

        await producerService.CallAsync(1);

        var value = await producerService.GetAsync();
        Assert.True(value == 1);
    }

    /// <summary>
    /// Tests the streaming.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TestStreamAsync()
    {
        var consumerService = _cluster.GrainFactory.GetGrain<IConsumerService>(Guid.Empty);
        var producerService = _cluster.GrainFactory.GetGrain<IProducerService>(Guid.Empty);

        await producerService.CallAsync(1);
        await Task.Delay(500);
        var otherSideValue = await consumerService.GetAsync();

        Assert.Equal(1, otherSideValue);
    }
}
