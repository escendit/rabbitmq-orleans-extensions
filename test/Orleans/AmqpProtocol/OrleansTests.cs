// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Tests;

using Collections;
using Fixtures;
using global::Orleans.Streams;
using global::Orleans.TestingHost;
using RabbitMQ.Tests.Grains;

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
    /// <param name="fixture">The cluster fixture.</param>
    public OrleansTests(ClusterFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _cluster = fixture.Cluster;
    }

    /// <summary>
    /// Start.
    /// </summary>
    [Fact]
    [IntegrationTest]
    public void ClusterIsUp()
    {
        Assert.NotNull(_cluster);
    }

    /// <summary>
    /// Test Client.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ClientActionAsync()
    {
        var producerService = _cluster.Client.GetGrain<IProducerService>(Guid.Empty);
        await producerService.CallAsync(1);
        var value = await producerService.GetAsync();
        Assert.Equal(1, value);
    }

    /// <summary>
    /// Test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task SiloActionAsync()
    {
        // Method intentionally left empty.
        var producerService = _cluster.GrainFactory.GetGrain<IProducerService>(Guid.Empty);

        await producerService.CallAsync(1);

        var value = await producerService.GetAsync();
        Assert.Equal(1, value);
    }

    /// <summary>
    /// Tests the streaming with client.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ClientStreamTestAsync()
    {
        var consumerService = _cluster.Client.GetGrain<IConsumerService>(Guid.Empty);
        var producerService = _cluster.Client.GetGrain<IProducerService>(Guid.Empty);

        await producerService.CallAsync(1);
        await Task.Delay(1500);
        var otherSideValue = await consumerService.GetAsync();

        Assert.Equal(1, otherSideValue);
    }

    /// <summary>
    /// Tests the streaming.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task SiloStreamTestAsync()
    {
        var consumerService = _cluster.GrainFactory.GetGrain<IConsumerService>(Guid.Empty);
        var producerService = _cluster.GrainFactory.GetGrain<IProducerService>(Guid.Empty);

        await producerService.CallAsync(1);
        await Task.Delay(1000);
        var otherSideValue = await consumerService.GetAsync();

        Assert.Equal(1, otherSideValue);
    }

    /// <summary>
    /// Direct Client Streaming.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ClientDirectStreamAsync()
    {
        var consumerService = _cluster.Client.GetGrain<IConsumerService>(Guid.Empty);
        var producerService = _cluster.Client.GetGrain<IProducerService>(Guid.Empty);
        var streamProvider = _cluster.Client.GetStreamProvider("client");
        var stream = streamProvider.GetStream<ProducerEvent>(Guid.Empty);
        await stream.SubscribeAsync(Observer);

        await producerService.CallAsync(1);
        await Task.Delay(2500);

        var newValue = await consumerService.GetAsync();
        Assert.Equal(1, newValue);
    }

    private static Task Observer(ProducerEvent @event, StreamSequenceToken seq)
    {
        Assert.Equal(1, @event.NewValue);
        return Task.CompletedTask;
    }
}
