// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Handlers;

using global::Orleans.Runtime;
using global::Orleans.Streams;
using Microsoft.Extensions.Logging;

/// <summary>
/// RabbitMQ Stream Failure Handler.
/// </summary>
internal partial class RabbitMqStreamDeliveryFailureHandler : IStreamFailureHandler
{
    private readonly ILogger _logger;
    private readonly QueueId _queueId;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqStreamDeliveryFailureHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="queueId">The queue id.</param>
    /// <param name="shouldFaultSubscriptionOnError">The value if it should fault on subscription error.</param>
    public RabbitMqStreamDeliveryFailureHandler(ILogger<RabbitMqStreamDeliveryFailureHandler> logger, QueueId queueId, bool shouldFaultSubscriptionOnError = false)
    {
        _logger = logger;
        _queueId = queueId;
        ShouldFaultSubsriptionOnError = shouldFaultSubscriptionOnError;
    }

    /// <inheritdoc/>
    public bool ShouldFaultSubsriptionOnError { get; }

    /// <inheritdoc />
    public Task OnDeliveryFailure(
        GuidId subscriptionId,
        string streamProviderName,
        StreamId streamIdentity,
        StreamSequenceToken sequenceToken)
    {
        LogDeliveryFailure(_queueId, subscriptionId.Guid, streamProviderName, streamIdentity, sequenceToken, ShouldFaultSubsriptionOnError);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnSubscriptionFailure(
        GuidId subscriptionId,
        string streamProviderName,
        StreamId streamIdentity,
        StreamSequenceToken sequenceToken)
    {
        LogSubscriptionFailure(_queueId, subscriptionId.Guid, streamProviderName, streamIdentity, sequenceToken, ShouldFaultSubsriptionOnError);
        return Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 1000,
        EventName = "Delivery Failure",
        Level = LogLevel.Warning,
        Message = "Delivery Failure with QueueId: {queueId}, Subscription: {subscriptionId}, Stream Provider: {streamProviderName}, Stream Id: {streamId}, Sequence Token: {streamSequenceToken} with {fault}")]
    private partial void LogDeliveryFailure(QueueId queueId, Guid subscriptionId, string streamProviderName, StreamId streamId, StreamSequenceToken streamSequenceToken, bool fault);

    [LoggerMessage(
        EventId = 1010,
        EventName = "Subscription Failure",
        Level = LogLevel.Warning,
        Message = "Subscription Failure with QueueId: {queueId}, Subscription: {subscriptionId}, Stream Provider: {streamProviderName}, Stream Id: {streamId}, Sequence Token: {streamSequenceToken} with {fault}")]
    private partial void LogSubscriptionFailure(QueueId queueId, Guid subscriptionId, string streamProviderName, StreamId streamId, StreamSequenceToken streamSequenceToken, bool fault);
}
