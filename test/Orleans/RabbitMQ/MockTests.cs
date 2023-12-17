// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace RabbitMQ.Tests;

using Escendit.Orleans.Streaming.RabbitMQ.Core;
using Escendit.Orleans.Streaming.RabbitMQ.Provider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Orleans.Runtime;
using Xunit.Categories;

/// <summary>
/// Mock Tests.
/// </summary>
public sealed class MockTests : IDisposable
{
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockTests"/> class.
    /// </summary>
    public MockTests()
    {
        _loggerFactory = GetLoggerFactory();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="MockTests"/> class.
    /// </summary>
    ~MockTests() => Dispose();

    /// <summary>
    /// Initial Adapter Factory Base.
    /// </summary>
    [Fact]
    [UnitTest]
    public void InitiateAdapterFactoryBase()
    {
        var adapterFactory = Substitute.For<AdapterFactoryBase>(GetLogger("test"));
        Assert.NotNull(adapterFactory);
    }

    /// <summary>
    /// Initial Adapter Receiver Base.
    /// </summary>
    [Fact]
    [UnitTest]
    public void InitiateAdapterReceiverBase()
    {
        var adapterReceiverBase = Substitute.For<AdapterReceiverBase>(GetLogger("test"));
        Assert.NotNull(adapterReceiverBase);
    }

    /// <summary>
    /// Case when service name is null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ThrowWhenCreateDefaultStreamQueueMapperNameIsNull()
    {
        var serviceCollection = new ServiceCollection();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        Assert.Throws<ArgumentNullException>(() => serviceProvider.CreateDefaultStreamQueueMapper(null));
    }

    /// <summary>
    /// Case when service name is null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ThrowWhenCreateDefaultStreamQueueMapperNameIsNotString()
    {
        var serviceCollection = new ServiceCollection();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        Assert.Throws<ArgumentException>(() => serviceProvider.CreateDefaultStreamQueueMapper(1));
    }

    /// <summary>
    /// Case when service name is null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ThrowWhenCreateDefaultQueueAdapterCacheNameIsNotString()
    {
        var serviceCollection = new ServiceCollection();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        Assert.Throws<ArgumentException>(() => serviceProvider.CreateDefaultQueueAdapterCache(1));
    }

    /// <summary>
    /// Operator Matching.
    /// </summary>
    [Fact]
    [UnitTest]
    public void OperatorMatching()
    {
        var left = new RabbitMqBatchContainer(
            StreamId.Create("test", 1),
            new List<object>(),
            new Dictionary<string, object>(),
            null,
            1);

        var right = new RabbitMqBatchContainer(
            StreamId.Create("test", 2),
            new List<object>(),
            new Dictionary<string, object>(),
            null,
            2);

        Assert.True(left != right);
        Assert.False(left == right);
        Assert.True(left < right);
        Assert.True(left <= right);
        Assert.False(left > right);
        Assert.False(left >= right);
    }

    /// <summary>
    /// Match With Left Null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CompareMatchLeftNull()
    {
#pragma warning disable CA1508
        var left = default(RabbitMqBatchContainer);
        var right = new RabbitMqBatchContainer(
            StreamId.Create("test", 2),
            new List<object>(),
            new Dictionary<string, object>(),
            null,
            2);

        Assert.False(left == right);
#pragma warning restore CA1508
    }

    /// <summary>
    /// Match With Right Null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CompareMatchRightNull()
    {
#pragma warning disable CA1508
        var left = new RabbitMqBatchContainer(
            StreamId.Create("test", 2),
            new List<object>(),
            new Dictionary<string, object>(),
            null,
            2);
        var right = default(RabbitMqBatchContainer);

        Assert.False(left == right);
#pragma warning restore CA1508
    }

    /// <summary>
    /// Get HashCode to be different than 0.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CompareHashCode()
    {
        var container = new RabbitMqBatchContainer(
            StreamId.Create("test", 2),
            new List<object>(),
            new Dictionary<string, object>(),
            null,
            2);
        Assert.NotEqual(0, container.GetHashCode());
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _loggerFactory.Dispose();
        GC.SuppressFinalize(this);
    }

    private static ILoggerFactory GetLoggerFactory()
    {
        return LoggerFactory.Create(_ => { });
    }

    private ILogger GetLogger(string loggerName)
    {
        return _loggerFactory.CreateLogger(loggerName);
    }
}
