// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

#pragma warning disable CA1812

namespace Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Provider;

using Configuration;
using Core;
using global::Orleans.Configuration;
using global::Orleans.Configuration.Overrides;
using global::Orleans.Providers.Streams.Common;
using global::Orleans.Serialization;
using global::Orleans.Streams;
using global::RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ConnectionOptions = Extensions.DependencyInjection.RabbitMQ.Abstractions.ConnectionOptions;

/// <summary>
/// AMQP Protocol Adapter Factory.
/// </summary>
internal partial class AmqpProtocolAdapterFactory : IQueueAdapterFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;
    private readonly string _name;
    private readonly IConnection _connection;
    private readonly IStreamQueueMapper _streamQueueMapper;
    private readonly IQueueAdapterCache _queueAdapterCache;
    private readonly QueueOptions _queueOptions;
    private readonly ClusterOptions _clusterOptions;
    private readonly Serializer<RabbitMqBatchContainer> _serializer;
    private readonly Func<QueueId, Task<IStreamFailureHandler>> _streamFailureHandlerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="AmqpProtocolAdapterFactory"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="streamQueueMapper">The stream queue mapper.</param>
    /// <param name="queueAdapterCache">The queue adapter cache.</param>
    /// <param name="queueOptions">The queue options.</param>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="streamFailureHandler">The stream failure handler.</param>
    public AmqpProtocolAdapterFactory(
        string name,
        IConnection connection,
        IStreamQueueMapper streamQueueMapper,
        IQueueAdapterCache queueAdapterCache,
        QueueOptions queueOptions,
        ClusterOptions clusterOptions,
        Serializer serializer,
        ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(streamQueueMapper);
        ArgumentNullException.ThrowIfNull(clusterOptions);
        ArgumentNullException.ThrowIfNull(serializer);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<AmqpProtocolAdapterFactory>();
        _name = name;
        _connection = connection;
        _queueOptions = queueOptions;
        _clusterOptions = clusterOptions;
        _serializer = serializer.GetSerializer<RabbitMqBatchContainer>();
        _streamQueueMapper = streamQueueMapper;
        _queueAdapterCache = queueAdapterCache;
        _streamFailureHandlerFactory = queueOptions.StreamFailureHandler;
    }

    /// <inheritdoc/>
    public Task<IQueueAdapter> CreateAdapter()
    {
        LogCreateAdapter(_name);

        return Task
            .FromResult<IQueueAdapter>(new AmqpProtocolAdapter(
                _name,
                _connection,
                _loggerFactory,
                _queueOptions,
                _clusterOptions,
                _serializer,
                _streamQueueMapper));
    }

    /// <inheritdoc/>
    public IQueueAdapterCache GetQueueAdapterCache()
    {
        LogGetQueueAdapterCache(_name);
        return _queueAdapterCache;
    }

    /// <inheritdoc/>
    public IStreamQueueMapper GetStreamQueueMapper()
    {
        LogGetStreamQueueMapper(_name);
        return _streamQueueMapper;
    }

    /// <inheritdoc/>
    public Task<IStreamFailureHandler> GetDeliveryFailureHandler(QueueId queueId)
    {
        LogGetDeliveryFailureHandler(_name, queueId);
        return _streamFailureHandlerFactory(queueId);
    }

    /// <summary>
    /// Create Queue Adapter Factory.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="name">The name.</param>
    /// <returns>The queue adapter factory.</returns>
    internal static AmqpProtocolAdapterFactory Create(IServiceProvider serviceProvider, object? name)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(name);

        if (name is not string factoryName)
        {
            throw new ArgumentException("invalid name");
        }

        var clusterOptions = serviceProvider.GetProviderClusterOptions(factoryName);
        var queueOptions = serviceProvider.GetOptionsByName<QueueOptions>(factoryName);
        var queueMapperOptions = serviceProvider.GetOptionsByName<HashRingStreamQueueMapperOptions>(factoryName);
        var connection = serviceProvider.GetRequiredOrleansServiceByName<IConnection>(factoryName);
        var streamQueueMapper = serviceProvider.GetOptionalOrleansServiceByName<IStreamQueueMapper>(factoryName);
        var adapterCache = serviceProvider.GetOptionalOrleansServiceByName<IQueueAdapterCache>(factoryName);

        streamQueueMapper ??= new HashRingBasedStreamQueueMapper(queueMapperOptions, factoryName);

        adapterCache ??= new SimpleQueueAdapterCache(
            new SimpleQueueCacheOptions(),
            factoryName,
            serviceProvider.GetRequiredService<ILoggerFactory>());

        return ActivatorUtilities
            .CreateInstance<AmqpProtocolAdapterFactory>(
                serviceProvider,
                name,
                connection,
                streamQueueMapper,
                adapterCache,
                queueOptions,
                clusterOptions.Value);
    }

    [LoggerMessage(
        EventId = 100,
        EventName = nameof(CreateAdapter),
        Level = LogLevel.Debug,
        Message = "Creating Queue Adapter for ProviderName: {name}")]
    private partial void LogCreateAdapter(string name);

    [LoggerMessage(
        EventId = 101,
        EventName = nameof(GetQueueAdapterCache),
        Level = LogLevel.Debug,
        Message = "Setting Queue Adapter Cache for ProviderName: {name}")]
    private partial void LogGetQueueAdapterCache(string name);

    [LoggerMessage(
        EventId = 102,
        EventName = nameof(GetStreamQueueMapper),
        Level = LogLevel.Debug,
        Message = "Getting Stream Queue Mapper for ProviderName: {name}")]
    private partial void LogGetStreamQueueMapper(string name);

    [LoggerMessage(
        EventId = 500,
        EventName = nameof(GetDeliveryFailureHandler),
        Level = LogLevel.Debug,
        Message = "Getting Delivery Failure Handler for ProviderName: {name}, QueueId: {queueId}")]
    private partial void LogGetDeliveryFailureHandler(string name, QueueId queueId);
}
