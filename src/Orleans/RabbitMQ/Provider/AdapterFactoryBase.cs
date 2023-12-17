// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Provider;

using global::Orleans.Streams;
using Microsoft.Extensions.Logging;

/// <inheritdoc />
public abstract partial class AdapterFactoryBase : IQueueAdapterFactory
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdapterFactoryBase"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    protected AdapterFactoryBase(ILogger logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public abstract Task<IQueueAdapter> CreateAdapter();

    /// <inheritdoc/>
    public abstract IQueueAdapterCache GetQueueAdapterCache();

    /// <inheritdoc/>
    public abstract IStreamQueueMapper GetStreamQueueMapper();

    /// <inheritdoc/>
    public abstract Task<IStreamFailureHandler> GetDeliveryFailureHandler(QueueId queueId);

    /// <summary>
    /// Log Create Adapter.
    /// </summary>
    /// <param name="name">The name.</param>
    [LoggerMessage(
        EventId = 100,
        EventName = nameof(CreateAdapter),
        Level = LogLevel.Debug,
        Message = "Creating Queue Adapter for ProviderName: {name}")]
    protected partial void LogCreateAdapter(string name);

    /// <summary>
    /// Log Get Queue Adapter Cache.
    /// </summary>
    /// <param name="name">The name.</param>
    [LoggerMessage(
        EventId = 101,
        EventName = nameof(GetQueueAdapterCache),
        Level = LogLevel.Debug,
        Message = "Setting Queue Adapter Cache for ProviderName: {name}")]
    protected partial void LogGetQueueAdapterCache(string name);

    /// <summary>
    /// Log Get Stream Queue Mapper.
    /// </summary>
    /// <param name="name">The name.</param>
    [LoggerMessage(
        EventId = 102,
        EventName = nameof(GetStreamQueueMapper),
        Level = LogLevel.Debug,
        Message = "Getting Stream Queue Mapper for ProviderName: {name}")]
    protected partial void LogGetStreamQueueMapper(string name);

    /// <summary>
    /// Log Get Delivery Failure Handler.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="queueId"></param>
    [LoggerMessage(
        EventId = 500,
        EventName = nameof(GetDeliveryFailureHandler),
        Level = LogLevel.Debug,
        Message = "Getting Delivery Failure Handler for ProviderName: {name}, QueueId: {queueId}")]
    protected partial void LogGetDeliveryFailureHandler(string name, QueueId queueId);
}
