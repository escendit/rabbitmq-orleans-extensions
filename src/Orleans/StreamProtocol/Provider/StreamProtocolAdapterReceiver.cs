// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.Provider;

using System.Threading.Channels;
using Core;
using global::Orleans.Configuration;
using global::Orleans.Serialization;
using global::Orleans.Streams;
using global::RabbitMQ.Stream.Client;
using Microsoft.Extensions.Logging;

/// <summary>
/// Stream Protocol Adapter Receiver.
/// </summary>
internal sealed partial class StreamProtocolAdapterReceiver : IQueueAdapterReceiver
{
    private readonly ILogger _logger;
    private readonly string _name;
    private readonly ClusterOptions _clusterOptions;
    private readonly QueueId _queueId;
    private readonly Serializer<RabbitMqBatchContainer> _serializer;
    private readonly StreamSystem _streamSystem;
    private readonly Channel<RabbitMqBatchContainer> _inboundChannel;
    private IConsumer? _consumer;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProtocolAdapterReceiver"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="queueId">The queue id.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="streamSystem">The stream system.</param>
    public StreamProtocolAdapterReceiver(
        string name,
        ClusterOptions clusterOptions,
        QueueId queueId,
        ILoggerFactory loggerFactory,
        Serializer<RabbitMqBatchContainer> serializer,
        StreamSystem streamSystem)
    {
        _name = name;
        _clusterOptions = clusterOptions;
        _queueId = queueId;
        _logger = loggerFactory.CreateLogger($"Escendit.Orleans.Streaming.RabbitMQ.Stream.{_queueId}");
        _serializer = serializer;
        _streamSystem = streamSystem;
        _inboundChannel = Channel.CreateBounded<RabbitMqBatchContainer>(16);
    }

    /// <inheritdoc/>
    public async Task Initialize(TimeSpan timeout)
    {
        LogInitialize(_name, _queueId);
        var streamName = NamingUtility.CreateNameForStream(_clusterOptions, _queueId);
        _consumer = await _streamSystem
            .CreateRawConsumer(new RawConsumerConfig(streamName)
            {
                MessageHandler = async (_, _, message) =>
                {
                    LogMessageHandlerIncomingMessage(_name, _queueId, message.Data.Size);
                    await _inboundChannel.Writer.WaitToWriteAsync(); // Wait for queue
                    var container = _serializer.Deserialize(message.Data.Contents);
                    await _inboundChannel.Writer.WriteAsync(container);
                },
            });
    }

    /// <inheritdoc/>
    public Task<IList<IBatchContainer>> GetQueueMessagesAsync(int maxCount)
    {
        LogGetQueueMessages(_name, _queueId, maxCount);
        var countdown = maxCount;
        var batchContainers = new List<IBatchContainer>(maxCount);

        while (countdown > 0)
        {
            Interlocked.Decrement(ref countdown);

            if (_inboundChannel.Reader.TryRead(out var item))
            {
                batchContainers.Add(item);
                continue;
            }

            // break the cycle if channel is exhausted.
            break;
        }

        return Task.FromResult<IList<IBatchContainer>>(batchContainers);
    }

    /// <inheritdoc/>
    public async Task MessagesDeliveredAsync(IList<IBatchContainer> messages)
    {
        ArgumentNullException.ThrowIfNull(messages);
        LogMessagesDelivered(_name, _queueId, messages.Count);

        var maxNumber = Convert.ToUInt64(messages.Max(p => p.SequenceToken.SequenceNumber));

        if (maxNumber > 0)
        {
            await _consumer!.StoreOffset(maxNumber);
        }
    }

    /// <inheritdoc/>
    public async Task Shutdown(TimeSpan timeout)
    {
        LogShutdown(_name, _queueId);

        if (_consumer is not null)
        {
            await _consumer.Close();
        }
    }

    [LoggerMessage(
        EventId = 100,
        EventName = "Log Initialize",
        Level = LogLevel.Debug,
        Message = "Initializing Receiver for ProviderName: {name}, QueueId: {queueId}")]
    private partial void LogInitialize(string name, QueueId queueId);

    [LoggerMessage(
        EventId = 101,
        EventName = "Log Message Handler Incoming Message",
        Level = LogLevel.Debug,
        Message = "Incoming Message for ProviderName: {name}, QueueId: {queueId}, Size: {size}")]
    private partial void LogMessageHandlerIncomingMessage(string name, QueueId queueId, int size);

    [LoggerMessage(
        EventId = 102,
        EventName = "Log Get Queue Messages",
        Level = LogLevel.Debug,
        Message = "Getting Queue Messages for ProviderName: {name}, QueueId {queueId}, MaxCount: {maxCount}")]
    private partial void LogGetQueueMessages(string name, QueueId queueId, int maxCount);

    [LoggerMessage(
        EventId = 103,
        EventName = "Log Messages Delivered",
        Level = LogLevel.Debug,
        Message = "Delivered Messages for ProviderName: {name}, QueueId: {queueId}, Count: {count}")]
    private partial void LogMessagesDelivered(string name, QueueId queueId, int count);

    [LoggerMessage(
        EventId = 104,
        EventName = "Log Shutdown",
        Level = LogLevel.Debug,
        Message = "Shutting down for ProviderName: {name}, QueueId: {queueId}")]
    private partial void LogShutdown(string name, QueueId queueId);
}
