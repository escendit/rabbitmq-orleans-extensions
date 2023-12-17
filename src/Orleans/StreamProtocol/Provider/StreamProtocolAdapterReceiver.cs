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
using RabbitMQ.Provider;

/// <summary>
/// Stream Protocol Adapter Receiver.
/// </summary>
internal sealed class StreamProtocolAdapterReceiver : AdapterReceiverBase
{
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
        : base(loggerFactory.CreateLogger($"Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.{queueId}"))
    {
        _name = name;
        _clusterOptions = clusterOptions;
        _queueId = queueId;
        _serializer = serializer;
        _streamSystem = streamSystem;
        _inboundChannel = Channel.CreateBounded<RabbitMqBatchContainer>(16);
    }

    /// <inheritdoc/>
    public override async Task Initialize(TimeSpan timeout)
    {
        LogInitialize(_name, _queueId);
        var streamName = NamingUtility.CreateNameForStream(_clusterOptions, _queueId);

        if (!await _streamSystem.StreamExists(streamName))
        {
            await _streamSystem.CreateStream(new StreamSpec(streamName));
        }

        _consumer = await _streamSystem
            .CreateRawConsumer(new RawConsumerConfig(streamName)
            {
                MessageHandler = async (_, context, message) =>
                {
                    LogMessageHandlerIncomingMessage(_name, _queueId, message.Data.Size);
                    await _inboundChannel.Writer.WaitToWriteAsync(); // Wait for queue
                    var container = _serializer.Deserialize(message.Data.Contents);
                    container.UpdateDeliveryTag(context.Offset);
                    await _inboundChannel.Writer.WriteAsync(container);
                },
            });
    }

    /// <inheritdoc/>
    public override Task<IList<IBatchContainer>> GetQueueMessagesAsync(int maxCount)
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
    public override Task MessagesDeliveredAsync(IList<IBatchContainer> messages)
    {
        ArgumentNullException.ThrowIfNull(messages);
        LogMessagesDelivered(_name, _queueId, messages.Count);

        var maxNumber = messages.Cast<RabbitMqBatchContainer>().Max(p => p.DeliveryTag);

        return maxNumber > 0 ? _consumer!.StoreOffset(maxNumber) : Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override async Task Shutdown(TimeSpan timeout)
    {
        LogShutdown(_name, _queueId);

        if (_consumer is not null)
        {
            await _consumer.Close();
        }
    }
}
