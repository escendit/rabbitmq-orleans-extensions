// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Queue;

using Core;
using global::Orleans.Configuration;
using global::Orleans.Serialization;
using global::Orleans.Streams;
using global::RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using Options;

/// <summary>
/// Default Queue Adapter Receiver.
/// </summary>
internal partial class DefaultQueueAdapterReceiver : IQueueAdapterReceiver
{
    private readonly string _name;
    private readonly RabbitQueueOptions _options;
    private readonly ClusterOptions _clusterOptions;
    private readonly QueueId _queueId;
    private readonly ILogger _logger;
    private readonly Serializer<RabbitBatchContainer> _serializer;
    private readonly IConnection _connection;
    private IModel? _model;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultQueueAdapterReceiver"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="options">The options.</param>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="queueId">The queue id.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="connection">The connection.</param>
    public DefaultQueueAdapterReceiver(
        string name,
        RabbitQueueOptions options,
        ClusterOptions clusterOptions,
        QueueId queueId,
        ILoggerFactory loggerFactory,
        Serializer<RabbitBatchContainer> serializer,
        IConnection connection)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(serializer);
        ArgumentNullException.ThrowIfNull(connection);
        _name = name;
        _options = options;
        _clusterOptions = clusterOptions;
        _queueId = queueId;
        _logger = loggerFactory.CreateLogger<DefaultQueueAdapterReceiver>();
        _serializer = serializer;
        _connection = connection;
    }

    /// <inheritdoc/>
    public Task Initialize(TimeSpan timeout)
    {
        LogInitialize(_name, _queueId);
        _model = _connection.CreateModel();
        var queueName = NamingUtility.CreateNameForQueue(_clusterOptions, _queueId);
        _model.ExchangeDeclare(queueName, ExchangeType.Direct, _options.IsDurable);
        _model.QueueDeclare(queueName, _options.IsDurable, false);
        _model.QueueBind(queueName, queueName, _options.RoutingKey);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<IList<IBatchContainer>> GetQueueMessagesAsync(int maxCount)
    {
        LogGetQueueMessages(_name, _queueId, maxCount);
        ArgumentNullException.ThrowIfNull(_model);
        var queueName = NamingUtility.CreateNameForQueue(_clusterOptions, _queueId);
        var batchContainers = new List<IBatchContainer>();

        while (batchContainers.Count < maxCount)
        {
            var response = _model.BasicGet(queueName, false);

            if (response is not null)
            {
                LogMessageHandlerIncomingMessage(_name, _queueId, response.Body.Length);
                var container = _serializer.Deserialize(response.Body);

                container.UpdateSequenceToken(
                    new RabbitStreamSequenceToken(
                        Convert.ToInt64(response.DeliveryTag)));

                batchContainers.Add(container);
                continue;
            }

            // break exhaustion.
            break;
        }

        return Task.FromResult<IList<IBatchContainer>>(batchContainers);
    }

    /// <inheritdoc/>
    public Task MessagesDeliveredAsync(IList<IBatchContainer> messages)
    {
        LogMessagesDelivered(_name, _queueId, messages.Count);
        ArgumentNullException.ThrowIfNull(_model);
        foreach (var batchContainer in messages)
        {
            _model.BasicAck(Convert.ToUInt64(batchContainer.SequenceToken.SequenceNumber), false);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task Shutdown(TimeSpan timeout)
    {
        LogShutdown(_name, _queueId);
        ArgumentNullException.ThrowIfNull(_model);
        if (!_model.IsClosed)
        {
            _model.Close();
        }

        return Task.CompletedTask;
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
