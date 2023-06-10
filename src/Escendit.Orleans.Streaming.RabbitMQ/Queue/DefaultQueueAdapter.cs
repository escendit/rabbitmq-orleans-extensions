// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Queue;

using Core;
using global::Orleans.Configuration;
using global::Orleans.Runtime;
using global::Orleans.Serialization;
using global::Orleans.Streams;
using global::RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using Options;

/// <summary>
/// Default Queue Adapter.
/// </summary>
internal partial class DefaultQueueAdapter : IQueueAdapter
{
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly RabbitQueueOptions _options;
    private readonly ClusterOptions _clusterOptions;
    private readonly Serializer<RabbitBatchContainer> _serializer;
    private readonly IConsistentRingStreamQueueMapper _consistentRingStreamQueueMapper;
    private readonly IModel _publisherChannel;
    private readonly IModel _consumerChannel;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultQueueAdapter"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="options">The options.</param>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="consistentRingStreamQueueMapper">The consistent ring stream queue mapper.</param>
    public DefaultQueueAdapter(
        string name,
        IConnection connection,
        ILoggerFactory loggerFactory,
        RabbitQueueOptions options,
        ClusterOptions clusterOptions,
        Serializer<RabbitBatchContainer> serializer,
        IConsistentRingStreamQueueMapper consistentRingStreamQueueMapper)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(serializer);
        ArgumentNullException.ThrowIfNull(consistentRingStreamQueueMapper);

        Name = name;
        IsRewindable = false;

        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<DefaultQueueAdapter>();
        _options = options;
        _clusterOptions = clusterOptions;
        _serializer = serializer;
        _consistentRingStreamQueueMapper = consistentRingStreamQueueMapper;
        _publisherChannel = connection.CreateModel();
        _consumerChannel = connection.CreateModel();
        _publisherChannel.ExchangeDeclare(NamingUtility.CreateNameForQueue(_clusterOptions, _options.Name), _options.Type, _options.IsDurable);
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc />
    public bool IsRewindable { get; }

    /// <inheritdoc />
    public StreamProviderDirection Direction => StreamProviderDirection.ReadWrite;

    /// <inheritdoc />
    public Task QueueMessageBatchAsync<T>(
        StreamId streamId,
        IEnumerable<T> events,
        StreamSequenceToken token,
        Dictionary<string, object> requestContext)
    {
        LogQueueMessageBatch(Name, streamId, token);

        if (token is not null)
        {
            throw new InvalidOperationException("stream sequence token is not supported");
        }

        var queueId = _consistentRingStreamQueueMapper.GetQueueForStream(streamId);
        var queueName = NamingUtility.CreateNameForQueue(_clusterOptions, queueId);
        var exchangeName = NamingUtility.CreateNameForQueue(_clusterOptions, _options.Name);

        var container = new RabbitBatchContainer(
            streamId,
            events.Cast<object>().ToList(),
            requestContext,
            new RabbitStreamSequenceToken(0));

        var data = _serializer
            .SerializeToArray(container);

        _publisherChannel.BasicPublish(exchangeName, queueName, false, null, data);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
    {
        LogCreateReceiver(Name, queueId);
        return new DefaultQueueAdapterReceiver(
            Name,
            _options,
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
