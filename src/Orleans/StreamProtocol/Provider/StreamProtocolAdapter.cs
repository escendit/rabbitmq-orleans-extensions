// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.Provider;

using Core;
using global::Orleans.Configuration;
using global::Orleans.Runtime;
using global::Orleans.Serialization;
using global::Orleans.Streams;
using global::RabbitMQ.Stream.Client;
using global::RabbitMQ.Stream.Client.Reliable;
using Microsoft.Extensions.Logging;

/// <summary>
/// Stream Protocol Adapter.
/// </summary>
internal sealed partial class StreamProtocolAdapter : IQueueAdapter
{
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ClusterOptions _clusterOptions;
    private readonly Serializer<RabbitMqBatchContainer> _serializer;
    private readonly IStreamQueueMapper _streamQueueMapper;
    private readonly StreamSystem _streamSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProtocolAdapter"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="streamQueueMapper">The consistent ring stream queue mapper.</param>
    /// <param name="streamSystem">The stream system.</param>
    public StreamProtocolAdapter(
        string name,
        ILoggerFactory loggerFactory,
        ClusterOptions clusterOptions,
        Serializer<RabbitMqBatchContainer> serializer,
        IStreamQueueMapper streamQueueMapper,
        StreamSystem streamSystem)
    {
        Name = name;
        IsRewindable = true;
        Direction = StreamProviderDirection.ReadWrite;

        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<StreamProtocolAdapter>();
        _clusterOptions = clusterOptions;
        _serializer = serializer;
        _streamQueueMapper = streamQueueMapper;

        _streamSystem = streamSystem;
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public bool IsRewindable { get; }

    /// <inheritdoc/>
    public StreamProviderDirection Direction { get; }

    /// <inheritdoc/>
    public async Task QueueMessageBatchAsync<T>(
        StreamId streamId,
        IEnumerable<T> events,
        StreamSequenceToken token,
        Dictionary<string, object> requestContext)
    {
        LogQueueMessageBatch(Name, streamId, token);

        if (token is not null)
        {
            throw new InvalidOperationException("stream sequence token is not supported.");
        }

        var queueId = _streamQueueMapper.GetQueueForStream(streamId);
        var streamName = NamingUtility.CreateNameForStream(_clusterOptions, queueId);

        if (!await _streamSystem.StreamExists(streamName))
        {
            await _streamSystem
                .CreateStream(
                    new StreamSpec(streamName));
        }

        var producer = await _streamSystem
            .CreateRawProducer(
                new RawProducerConfig(streamName),
                _loggerFactory.CreateLogger<Producer>());

        var lastPublishingId = await producer.GetLastPublishingId();

        var container = new RabbitMqBatchContainer(
            streamId,
            events.Cast<object>().ToList(),
            requestContext,
            new RabbitMqStreamSequenceToken(lastPublishingId));

        var data = _serializer
            .SerializeToArray(container);

        await producer
            .Send(lastPublishingId, new Message(data));
    }

    /// <inheritdoc/>
    public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
    {
        return new StreamProtocolAdapterReceiver(Name, _clusterOptions, queueId, _loggerFactory, _serializer, _streamSystem);
    }

    [LoggerMessage(
        EventId = 100,
        EventName = nameof(QueueMessageBatchAsync),
        Level = LogLevel.Debug,
        Message = "Queueing Message Batch for ProviderName: {name}, StreamId: {streamId}, StreamSequenceToken: {sequenceToken}")]
    private partial void LogQueueMessageBatch(string name, StreamId streamId, StreamSequenceToken sequenceToken);
}
