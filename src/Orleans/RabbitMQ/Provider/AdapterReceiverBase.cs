// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Provider;

using global::Orleans.Streams;
using Microsoft.Extensions.Logging;

/// <inheritdoc />
public abstract partial class AdapterReceiverBase : IQueueAdapterReceiver
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdapterReceiverBase"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    protected AdapterReceiverBase(ILogger logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public abstract Task Initialize(TimeSpan timeout);

    /// <inheritdoc />
    public abstract Task<IList<IBatchContainer>> GetQueueMessagesAsync(int maxCount);

    /// <inheritdoc/>
    public abstract Task MessagesDeliveredAsync(IList<IBatchContainer> messages);

    /// <inheritdoc/>
    public abstract Task Shutdown(TimeSpan timeout);

    /// <summary>
    /// Log Initialize.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="queueId">The queue id.</param>
    [LoggerMessage(
        EventId = 100,
        EventName = "Log Initialize",
        Level = LogLevel.Debug,
        Message = "Initializing Receiver for ProviderName: {name}, QueueId: {queueId}")]
    protected partial void LogInitialize(string name, QueueId queueId);

    /// <summary>
    /// Log Message Handler Incoming Message.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="queueId">The queue id.</param>
    /// <param name="size">The size.</param>
    [LoggerMessage(
        EventId = 101,
        EventName = "Log Message Handler Incoming Message",
        Level = LogLevel.Debug,
        Message = "Incoming Message for ProviderName: {name}, QueueId: {queueId}, Size: {size}")]
    protected partial void LogMessageHandlerIncomingMessage(string name, QueueId queueId, int size);

    /// <summary>
    /// Log Get Queue Messages.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="queueId">The queue id.</param>
    /// <param name="maxCount">The max count.</param>
    [LoggerMessage(
        EventId = 102,
        EventName = "Log Get Queue Messages",
        Level = LogLevel.Trace,
        Message = "Getting Queue Messages for ProviderName: {name}, QueueId {queueId}, MaxCount: {maxCount}")]
    protected partial void LogGetQueueMessages(string name, QueueId queueId, int maxCount);

    /// <summary>
    /// Log Messages Delivered.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="queueId">The queue id.</param>
    /// <param name="count">The count.</param>
    [LoggerMessage(
        EventId = 103,
        EventName = "Log Messages Delivered",
        Level = LogLevel.Debug,
        Message = "Delivered Messages for ProviderName: {name}, QueueId: {queueId}, Count: {count}")]
    protected partial void LogMessagesDelivered(string name, QueueId queueId, int count);

    /// <summary>
    /// Log Shutdown.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="queueId">The queue id.</param>
    [LoggerMessage(
        EventId = 104,
        EventName = "Log Shutdown",
        Level = LogLevel.Debug,
        Message = "Shutting down for ProviderName: {name}, QueueId: {queueId}")]
    protected partial void LogShutdown(string name, QueueId queueId);
}
