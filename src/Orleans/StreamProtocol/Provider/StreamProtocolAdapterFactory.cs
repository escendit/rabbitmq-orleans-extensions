// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.Provider;

using Configuration;
using Core;
using global::Orleans.Configuration;
using global::Orleans.Configuration.Overrides;
using global::Orleans.Serialization;
using global::Orleans.Streams;
using global::RabbitMQ.Stream.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Provider;

/// <summary>
/// Stream Protocol Adapter Factory.
/// </summary>
public sealed class StreamProtocolAdapterFactory : AdapterFactoryBase
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly string _name;
    private readonly StreamSystem _streamSystem;
    private readonly ClusterOptions _clusterOptions;
    private readonly Serializer<RabbitMqBatchContainer> _serializer;
    private readonly IStreamQueueMapper _streamQueueMapper;
    private readonly IQueueAdapterCache _queueAdapterCache;
    private readonly Func<QueueId, Task<IStreamFailureHandler>> _streamFailureHandlerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProtocolAdapterFactory"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="streamSystem">The stream system.</param>
    /// <param name="streamQueueMapper">stream queue mapper.</param>
    /// <param name="queueAdapterCache">The queue adapter cache.</param>
    /// <param name="streamOptions">The stream options.</param>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public StreamProtocolAdapterFactory(
        string name,
        StreamSystem streamSystem,
        IStreamQueueMapper streamQueueMapper,
        IQueueAdapterCache queueAdapterCache,
        StreamOptions streamOptions,
        ClusterOptions clusterOptions,
        Serializer serializer,
        ILoggerFactory loggerFactory)
        : base(loggerFactory.CreateLogger<StreamProtocolAdapterFactory>())
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(streamQueueMapper);
        ArgumentNullException.ThrowIfNull(queueAdapterCache);
        ArgumentNullException.ThrowIfNull(streamOptions);
        ArgumentNullException.ThrowIfNull(clusterOptions);
        ArgumentNullException.ThrowIfNull(serializer);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        _loggerFactory = loggerFactory;
        _name = name;
        _streamSystem = streamSystem;
        _streamQueueMapper = streamQueueMapper;
        _queueAdapterCache = queueAdapterCache;
        _clusterOptions = clusterOptions;
        _serializer = serializer.GetSerializer<RabbitMqBatchContainer>();
        _streamFailureHandlerFactory = streamOptions.StreamFailureHandler;
    }

    /// <inheritdoc />
    public override async Task<IQueueAdapter> CreateAdapter()
    {
        LogCreateAdapter(_name);
        return new StreamProtocolAdapter(_name, _loggerFactory, _clusterOptions, _serializer, _streamQueueMapper, _streamSystem);
    }

    /// <inheritdoc />
    public override IQueueAdapterCache GetQueueAdapterCache()
    {
        LogGetQueueAdapterCache(_name);
        return _queueAdapterCache;
    }

    /// <inheritdoc />
    public override IStreamQueueMapper GetStreamQueueMapper()
    {
        LogGetStreamQueueMapper(_name);
        return _streamQueueMapper;
    }

    /// <inheritdoc />
    public override Task<IStreamFailureHandler> GetDeliveryFailureHandler(QueueId queueId)
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
    internal static StreamProtocolAdapterFactory Create(IServiceProvider serviceProvider, object? name)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(name);

        if (name is not string factoryName)
        {
            throw new ArgumentException("invalid name");
        }

        var clusterOptions = serviceProvider.GetProviderClusterOptions(factoryName);
        var streamSystem = serviceProvider.GetRequiredOrleansServiceByName<StreamSystem>(factoryName);
        var streamQueueMapper = serviceProvider.GetRequiredOrleansServiceByName<IStreamQueueMapper>(factoryName);
        var queueAdapterCache = serviceProvider.GetRequiredOrleansServiceByName<IQueueAdapterCache>(factoryName);
        var streamOptions = serviceProvider.GetOptionsByName<StreamOptions>(factoryName);
        return ActivatorUtilities.CreateInstance<StreamProtocolAdapterFactory>(
            serviceProvider,
            name,
            streamSystem,
            streamQueueMapper,
            queueAdapterCache,
            streamOptions,
            clusterOptions.Value);
    }
}
