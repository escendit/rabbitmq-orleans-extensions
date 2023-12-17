// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Tests;

using AmqpProtocol.Configuration;
using Core;
using global::Orleans.Configuration;
using global::Orleans.Runtime;
using global::Orleans.Serialization;
using global::Orleans.Serialization.Codecs;
using global::Orleans.Serialization.Configuration;
using global::Orleans.Serialization.Serializers;
using global::Orleans.Serialization.Session;
using global::Orleans.Serialization.TypeSystem;
using global::Orleans.Streams;
using global::RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Provider;
using RabbitMQ.Tests.Grains;

/// <summary>
/// Mock Tests.
/// </summary>
public sealed class MockTests : IDisposable
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockTests"/> class.
    /// </summary>
    public MockTests()
    {
        _loggerFactory = new NullLoggerFactory();
        var serviceCollection = new ServiceCollection();
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="MockTests"/> class.
    /// </summary>
    ~MockTests() => Dispose(false);

    /// <summary>
    /// Create Amqp Protocol Adapter.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CreateAmqpProtocolAdapter()
    {
        var testAdapter = BuildProtocolAdapter();
        Assert.NotNull(testAdapter);
    }

    /// <summary>
    /// Get Amqp Protocol Adapter Name.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetAmqpProtocolAdapterName()
    {
        var testAdapter = BuildProtocolAdapter();
        Assert.NotNull(testAdapter.Name);
    }

    /// <summary>
    /// Get Amqp Protocol Adapter IsRewindable.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetAmqpProtocolAdapterIsRewindable()
    {
        var testAdapter = BuildProtocolAdapter();
        Assert.False(testAdapter.IsRewindable);
    }

    /// <summary>
    /// Get Amqp Protocol Adapter Direction.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetAmqpProtocolAdapterDirection()
    {
        var testAdapter = BuildProtocolAdapter();
        Assert.Equal(StreamProviderDirection.ReadWrite, testAdapter.Direction);
    }

    /// <summary>
    /// Create Receiver.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CreateReceiver()
    {
        var testAdapter = BuildProtocolAdapter();
        Assert.NotNull(testAdapter.CreateReceiver(QueueId.GetQueueId("test", 0, 0)));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _loggerFactory.Dispose();
    }

    private AmqpProtocolAdapter BuildProtocolAdapter()
    {
        var options = Options.Create(new TypeManifestOptions());
        var connection = Substitute.For<IConnection>();
        var typeConverter = new TypeConverter(
            new List<ITypeConverter>(),
            new List<ITypeNameFilter>(),
            new List<ITypeFilter>(),
            options,
            new CachedTypeResolver());
        var typeCodec = new TypeCodec(typeConverter);
        var wellKnownTypes = new WellKnownTypeCollection(options);
        var codecProvider = new CodecProvider(_serviceProvider, options);
        var serializerSessionPool = new SerializerSessionPool(typeCodec, wellKnownTypes, codecProvider);
        var fieldCodec = Substitute.For<IFieldCodec<RabbitMqBatchContainer>>();
        var serializer = new Serializer<RabbitMqBatchContainer>(fieldCodec, serializerSessionPool);

        var testAdapter = new AmqpProtocolAdapter(
            "test",
            connection,
            _loggerFactory,
            new QueueOptions(),
            new ClusterOptions(),
            serializer,
            new HashRingBasedStreamQueueMapper(new HashRingStreamQueueMapperOptions(), "test"));

        return testAdapter;
    }
}
