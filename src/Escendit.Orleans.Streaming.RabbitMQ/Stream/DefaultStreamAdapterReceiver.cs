// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Stream;

using System.Threading.Channels;
using Core;
using global::Orleans.Serialization;
using global::Orleans.Streams;
using global::RabbitMQ.Stream.Client;
using Microsoft.Extensions.Logging;

/// <summary>
/// Default Stream Adapter Receiver.
/// </summary>
internal class DefaultStreamAdapterReceiver : IQueueAdapterReceiver
{
    private readonly string _name;
    private readonly QueueId _queueId;
    private readonly ILoggerFactory _loggerFactory;
    private readonly Serializer<RabbitBatchContainer> _serializer;
    private readonly StreamSystem _streamSystem;
    private readonly Channel<RabbitBatchContainer> _inboundChannel;
    private IConsumer? _consumer;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultStreamAdapterReceiver"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="queueId">The queue id.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="streamSystem">The stream system.</param>
    public DefaultStreamAdapterReceiver(
        string name,
        QueueId queueId,
        ILoggerFactory loggerFactory,
        Serializer<RabbitBatchContainer> serializer,
        StreamSystem streamSystem)
    {
        _name = name;
        _queueId = queueId;
        _loggerFactory = loggerFactory;
        _serializer = serializer;
        _streamSystem = streamSystem;
        _inboundChannel = Channel.CreateBounded<RabbitBatchContainer>(16);
    }

    /// <inheritdoc />
    public async Task Initialize(TimeSpan timeout)
    {
        var logger = _loggerFactory.CreateLogger($"Escendit.Orleans.Streaming.RabbitMQ:Queue:{_queueId}");
        _consumer = await _streamSystem
            .CreateRawConsumer(new RawConsumerConfig(_queueId.ToString())
            {
                MessageHandler = async (consumer, context, message) =>
                {
                    logger.LogInformation("Incoming Message: {Message} with {Size}", message.Data.Contents.ToString(), message.Data.Size.ToString());
                    await _inboundChannel.Writer.WaitToWriteAsync();
                    var container = _serializer.Deserialize(message.Data.Contents);
                    await _inboundChannel.Writer.WriteAsync(container);
                },
            });
    }

    /// <inheritdoc />
    public Task<IList<IBatchContainer>> GetQueueMessagesAsync(int maxCount)
    {
        var countdown = maxCount;
        var batchContainers = new List<IBatchContainer>(maxCount);

        while (countdown > 0)
        {
            Interlocked.Decrement(ref countdown);

            if (_inboundChannel.Reader.TryRead(out var item))
            {
                batchContainers.Add(item);
            }
        }

        return Task.FromResult<IList<IBatchContainer>>(batchContainers);
    }

    /// <inheritdoc />
    public async Task MessagesDeliveredAsync(IList<IBatchContainer> messages)
    {
        ArgumentNullException.ThrowIfNull(messages);

        ulong maxNumber = Convert.ToUInt64(messages.Max(p => p.SequenceToken.SequenceNumber));

        if (maxNumber > 0)
        {
            await _consumer!.StoreOffset(maxNumber);
        }
    }

    /// <inheritdoc />
    public Task Shutdown(TimeSpan timeout)
    {
        return _consumer!.Close();
    }
}
