// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Provider;

using Configuration;
using Core;
using global::Orleans.Configuration;
using global::Orleans.Serialization;
using global::Orleans.Streams;
using global::RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using RabbitMQ.Provider;

/// <summary>
/// Escendit.Orleans.Streaming.RabbitMQ.Tests AMQP Protocol Adapter Receiver.
/// </summary>
internal sealed class AmqpProtocolAdapterReceiver : AdapterReceiverBase
{
    private readonly string _name;
    private readonly QueueOptions _queueOptions;
    private readonly ClusterOptions _clusterOptions;
    private readonly QueueId _queueId;
    private readonly Serializer<RabbitMqBatchContainer> _serializer;
    private readonly IModel _channel;

    /// <summary>
    /// Initializes a new instance of the <see cref="AmqpProtocolAdapterReceiver"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="queueOptions">The options.</param>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="queueId">The queue id.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="channel">The channel.</param>
    public AmqpProtocolAdapterReceiver(
        string name,
        QueueOptions queueOptions,
        ClusterOptions clusterOptions,
        QueueId queueId,
        ILoggerFactory loggerFactory,
        Serializer<RabbitMqBatchContainer> serializer,
        IModel channel)
        : base(loggerFactory.CreateLogger($"Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.{queueId}"))
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(serializer);
        ArgumentNullException.ThrowIfNull(channel);
        _name = name;
        _queueOptions = queueOptions;
        _clusterOptions = clusterOptions;
        _queueId = queueId;
        _serializer = serializer;
        _channel = channel;
    }

    /// <inheritdoc/>
    public override Task Initialize(TimeSpan timeout)
    {
        LogInitialize(_name, _queueId);
        var queueName = NamingUtility.CreateNameForQueue(_clusterOptions, _queueId);
        var exchangeName = NamingUtility.CreateNameForQueue(_clusterOptions, _queueOptions.Name);
        _channel.QueueDeclare(queueName, _queueOptions.IsDurable, _queueOptions.IsExclusive);
        _channel.QueueBind(queueName, exchangeName, queueName);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override Task<IList<IBatchContainer>> GetQueueMessagesAsync(int maxCount)
    {
        LogGetQueueMessages(_name, _queueId, maxCount);
        ArgumentNullException.ThrowIfNull(_channel);
        var queueName = NamingUtility.CreateNameForQueue(_clusterOptions, _queueId);
        var batchContainers = new List<IBatchContainer>();

        while (batchContainers.Count < maxCount)
        {
            var response = _channel.BasicGet(queueName, false);

            if (response is not null)
            {
                LogMessageHandlerIncomingMessage(_name, _queueId, response.Body.Length);
                var container = _serializer.Deserialize(response.Body);
                container.UpdateDeliveryTag(response.DeliveryTag);
                batchContainers.Add(container);
                continue;
            }

            // break exhaustion.
            break;
        }

        return Task.FromResult<IList<IBatchContainer>>(batchContainers);
    }

    /// <inheritdoc/>
    public override Task MessagesDeliveredAsync(IList<IBatchContainer> messages)
    {
        LogMessagesDelivered(_name, _queueId, messages.Count);

        foreach (var deliveryTag in messages.Cast<RabbitMqBatchContainer>().Select(s => s.DeliveryTag))
        {
            if (deliveryTag.HasValue)
            {
                _channel.BasicAck(deliveryTag.Value, false);
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override Task Shutdown(TimeSpan timeout)
    {
        LogShutdown(_name, _queueId);
        return Task.CompletedTask;
    }
}
