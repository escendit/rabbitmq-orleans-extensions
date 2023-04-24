// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Stream;

using Core;
using global::Orleans.Runtime;
using global::Orleans.Serialization;
using global::Orleans.Streams;
using global::RabbitMQ.Stream.Client;
using global::RabbitMQ.Stream.Client.Reliable;
using Microsoft.Extensions.Logging;
using Options;

/// <summary>
/// Default Stream Adapter.
/// </summary>
internal partial class DefaultStreamAdapter : IQueueAdapter
{
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly RabbitStreamOptions _options;
    private readonly Serializer<RabbitBatchContainer> _serializer;
    private readonly IConsistentRingStreamQueueMapper _consistentRingStreamQueueMapper;
    private readonly StreamSystem _streamSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultStreamAdapter"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="options">The options.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="consistentRingStreamQueueMapper">The consistent ring stream queue mapper.</param>
    /// <param name="streamSystem">The stream system.</param>
    public DefaultStreamAdapter(
        string name,
        ILoggerFactory loggerFactory,
        RabbitStreamOptions options,
        Serializer<RabbitBatchContainer> serializer,
        IConsistentRingStreamQueueMapper consistentRingStreamQueueMapper,
        StreamSystem streamSystem)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<DefaultStreamAdapter>();
        _options = options;
        _serializer = serializer;
        _consistentRingStreamQueueMapper = consistentRingStreamQueueMapper;
        Name = name;
        IsRewindable = true;
        _streamSystem = streamSystem;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public bool IsRewindable { get; }

    /// <inheritdoc />
    public StreamProviderDirection Direction => StreamProviderDirection.ReadWrite;

    /// <inheritdoc />
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

        var queueId = _consistentRingStreamQueueMapper.GetQueueForStream(streamId);
        var streamName = NamingUtility.CreateNameForStream(Name, queueId);

        await _streamSystem
            .CreateStream(
                new StreamSpec(streamName));

        var producer = await _streamSystem
            .CreateRawProducer(
                new RawProducerConfig(streamName),
                _loggerFactory.CreateLogger<Producer>());

        var lastPublishingId = await producer.GetLastPublishingId();
        var lastPublishingIdLong = Convert.ToInt64(lastPublishingId);

        var container = new RabbitBatchContainer(
            streamId,
            events.Cast<object>().ToList(),
            requestContext,
            new RabbitStreamSequenceToken(lastPublishingIdLong));

        var data = _serializer
            .SerializeToArray(container);

        await producer
            .Send(lastPublishingId, new Message(data));
    }

    /// <inheritdoc/>
    public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
    {
        return new DefaultStreamAdapterReceiver(Name, queueId, _loggerFactory, _serializer, _streamSystem);
    }

    [LoggerMessage(
        EventId = 100,
        EventName = nameof(QueueMessageBatchAsync),
        Level = LogLevel.Debug,
        Message = "Queueing Message Batch for ProviderName: {name}, StreamId: {streamId}, StreamSequenceToken: {sequenceToken}")]
    private partial void LogQueueMessageBatch(string name, StreamId streamId, StreamSequenceToken sequenceToken);
}
