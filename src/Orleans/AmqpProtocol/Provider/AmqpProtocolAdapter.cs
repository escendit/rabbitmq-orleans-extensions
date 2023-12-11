// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Provider;

using Configuration;
using Core;
using global::Orleans.Configuration;
using global::Orleans.Runtime;
using global::Orleans.Serialization;
using global::Orleans.Streams;
using global::RabbitMQ.Client;
using Microsoft.Extensions.Logging;

/// <summary>
/// Escendit.Orleans.Streaming.RabbitMQ.Tests AMQP Protocol Adapter.
/// </summary>
internal partial class AmqpProtocolAdapter : IQueueAdapter
{
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly QueueOptions _queueOptions;
    private readonly ClusterOptions _clusterOptions;
    private readonly Serializer<RabbitMqBatchContainer> _serializer;
    private readonly IStreamQueueMapper _streamQueueMapper;
    private readonly IModel _publisherChannel;
    private readonly IModel _consumerChannel;

    /// <summary>
    /// Initializes a new instance of the <see cref="AmqpProtocolAdapter"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="queueOptions">The queue options.</param>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="streamQueueMapper">The stream queue mapper.</param>
    public AmqpProtocolAdapter(
        string name,
        IConnection connection,
        ILoggerFactory loggerFactory,
        QueueOptions queueOptions,
        ClusterOptions clusterOptions,
        Serializer<RabbitMqBatchContainer> serializer,
        IStreamQueueMapper streamQueueMapper)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(queueOptions);
        ArgumentNullException.ThrowIfNull(serializer);
        ArgumentNullException.ThrowIfNull(streamQueueMapper);

        Name = name;
        IsRewindable = false;
        Direction = StreamProviderDirection.ReadWrite;

        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<AmqpProtocolAdapter>();
        _queueOptions = queueOptions;
        _clusterOptions = clusterOptions;
        _serializer = serializer;
        _streamQueueMapper = streamQueueMapper;
        _publisherChannel = connection.CreateModel();
        _consumerChannel = connection.CreateModel();
        _publisherChannel.ExchangeDeclare(NamingUtility.CreateNameForQueue(_clusterOptions, _queueOptions.Name), _queueOptions.Type, _queueOptions.IsDurable);
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public bool IsRewindable { get; }

    /// <inheritdoc/>
    public StreamProviderDirection Direction { get; }

    /// <inheritdoc/>
    public Task QueueMessageBatchAsync<T>(
        StreamId streamId,
        IEnumerable<T> events,
        StreamSequenceToken token,
        Dictionary<string, object> requestContext)
    {
        ArgumentNullException.ThrowIfNull(streamId);
        ArgumentNullException.ThrowIfNull(events);
        ArgumentNullException.ThrowIfNull(requestContext);
        LogQueueMessageBatch(Name, streamId, token);

        var queueId = _streamQueueMapper.GetQueueForStream(streamId);
        var queueName = NamingUtility.CreateNameForQueue(_clusterOptions, queueId);
        var exchangeName = NamingUtility.CreateNameForQueue(_clusterOptions, _queueOptions.Name);

        var container = new RabbitMqBatchContainer(
            streamId,
            events.Cast<object>().ToList(),
            requestContext,
            new RabbitMqStreamSequenceToken(token));

        var data = _serializer
            .SerializeToArray(container);

        _publisherChannel.BasicPublish(exchangeName, queueName, false, null, data);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
    {
        LogCreateReceiver(Name, queueId);
        return new AmqpProtocolAdapterReceiver(
            Name,
            _queueOptions,
            _clusterOptions,
            queueId,
            _loggerFactory,
            _serializer,
            _consumerChannel);
    }

    [LoggerMessage(
        EventId = 100,
        EventName = "Queue Message Batch",
        Level = LogLevel.Debug,
        Message = "Queueing Message Batch for ProviderName: {name}, StreamId: {streamId}, StreamSequenceToken: {sequenceToken}")]
    private partial void LogQueueMessageBatch(string name, StreamId streamId, StreamSequenceToken sequenceToken);

    [LoggerMessage(
        EventId = 101,
        EventName = "Create Receiver",
        Level = LogLevel.Debug,
        Message = "Creating Receiver for ProviderName: {name}, QueueId: {queueId}")]
    private partial void LogCreateReceiver(string name, QueueId queueId);
}
