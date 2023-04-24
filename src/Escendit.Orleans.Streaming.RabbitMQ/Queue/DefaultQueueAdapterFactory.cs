// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Queue;

using Core;
using global::Orleans.Configuration;
using global::Orleans.Configuration.Overrides;
using global::Orleans.Providers.Streams.Common;
using global::Orleans.Serialization;
using global::Orleans.Streams;
using global::RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Options;
using Stream;

#pragma warning disable CA1812

/// <summary>
/// Default Queue Adapter Factory.
/// </summary>
internal partial class DefaultQueueAdapterFactory : IQueueAdapterFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;
    private readonly string _name;
    private readonly RabbitQueueOptions _options;
    private readonly ClusterOptions _clusterOptions;
    private readonly Serializer<RabbitBatchContainer> _serializer;
    private readonly IConsistentRingStreamQueueMapper _streamQueueMapper;
    private readonly IQueueAdapterCache _queueAdapterCache;
    private readonly Func<QueueId, Task<IStreamFailureHandler>> _streamFailureHandlerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultQueueAdapterFactory"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="options">The options.</param>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public DefaultQueueAdapterFactory(
        string name,
        RabbitQueueOptions options,
        ClusterOptions clusterOptions,
        Serializer serializer,
        ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(clusterOptions);
        ArgumentNullException.ThrowIfNull(serializer);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<DefaultStreamAdapterFactory>();
        _name = name;
        _options = options;
        _clusterOptions = clusterOptions;
        _serializer = serializer.GetSerializer<RabbitBatchContainer>();
        _streamQueueMapper =
            new HashRingBasedStreamQueueMapper(
                new HashRingStreamQueueMapperOptions
                {
                    TotalQueueCount = _options.TotalQueueCount,
                },
                _name);
        _queueAdapterCache = new SimpleQueueAdapterCache(
            new SimpleQueueCacheOptions
            {
                CacheSize = _options.CacheSize,
            },
            _name,
            _loggerFactory);
        _streamFailureHandlerFactory = _options.StreamFailureHandlerFactory;
    }

    /// <inheritdoc />
    public Task<IQueueAdapter> CreateAdapter()
    {
        LogCreateAdapter(_name);
        var connectionFactory = new ConnectionFactory()
        {
            ClientProvidedName = _options.ClientProvidedName,
            HostName = _options.Endpoints.FirstOrDefault()?.HostName,
            Port = _options.Endpoints.FirstOrDefault()?.Port ?? 5672,
            Password = _options.Password,
            UserName = _options.UserName,
            VirtualHost = _options.VirtualHost,
        };

        return Task
            .FromResult<IQueueAdapter>(new DefaultQueueAdapter(
                _name,
                _loggerFactory,
                _options,
                _serializer,
                _streamQueueMapper,
                connectionFactory));
    }

    /// <inheritdoc />
    public IQueueAdapterCache GetQueueAdapterCache()
    {
        LogGetQueueAdapterCache(_name);
        return _queueAdapterCache;
    }

    /// <inheritdoc />
    public IStreamQueueMapper GetStreamQueueMapper()
    {
        LogGetStreamQueueMapper(_name);
        return _streamQueueMapper;
    }

    /// <inheritdoc />
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
    internal static DefaultQueueAdapterFactory Create(IServiceProvider serviceProvider, string name)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(name);
        var options = serviceProvider.GetOptionsByName<RabbitQueueOptions>(name);
        var clusterOptions = serviceProvider.GetProviderClusterOptions(name);
        return ActivatorUtilities
            .CreateInstance<DefaultQueueAdapterFactory>(
                serviceProvider,
                name,
                options,
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
