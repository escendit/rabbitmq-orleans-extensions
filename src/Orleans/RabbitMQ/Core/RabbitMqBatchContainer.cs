// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Core;

using System.Text.Json.Serialization;
using global::Orleans.Runtime;
using global::Orleans.Streams;

/// <summary>
/// Escendit.Orleans.Streaming.RabbitMQ.Tests Batch Container.
/// </summary>
[Serializable]
[GenerateSerializer]
[Alias("rabbitmq-batch-container")]
internal class RabbitMqBatchContainer : IBatchContainer, IComparable<RabbitMqBatchContainer>
{
    [Id(0)]
    [JsonPropertyName("events")]
    private readonly ICollection<object> _events;

    [Id(1)]
    [JsonPropertyName("requestContext")]
    private readonly Dictionary<string, object>? _requestContext;

    [Id(2)]
    [JsonPropertyName("sequenceToken")]
    private RabbitMqStreamSequenceToken? _sequenceToken;

    [Id(4)]
    [JsonPropertyName("deliveryTag")]
    private ulong? _deliveryTag;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqBatchContainer"/> class.
    /// </summary>
    /// <param name="streamId">The stream id.</param>
    /// <param name="events">The events.</param>
    /// <param name="requestContext">The request context.</param>
    /// <param name="sequenceToken">The sequence token.</param>
    /// <param name="deliveryTag">The delivery tag.</param>
    [JsonConstructor]
    public RabbitMqBatchContainer(
        StreamId streamId,
        ICollection<object> events,
        Dictionary<string, object> requestContext,
        RabbitMqStreamSequenceToken? sequenceToken,
        ulong? deliveryTag = default)
    {
        StreamId = streamId;
        _events = events;
        _requestContext = requestContext;
        _sequenceToken = sequenceToken;
        _deliveryTag = deliveryTag;
    }

    /// <inheritdoc />
    [JsonIgnore]
    public StreamSequenceToken? SequenceToken => _sequenceToken;

    /// <inheritdoc />
    [Id(3)]
    [JsonPropertyName("streamId")]
    public StreamId StreamId { get; }

    /// <summary>
    /// Gets the delivery tag.
    /// </summary>
    /// <value>The delivery tag.</value>
    [JsonIgnore]
    public ulong DeliveryTag => _deliveryTag ?? 0;

    public static bool operator ==(RabbitMqBatchContainer? left, RabbitMqBatchContainer? right)
    {
        return left?.Equals(right) ?? ReferenceEquals(left, right);
    }

    public static bool operator !=(RabbitMqBatchContainer? left, RabbitMqBatchContainer? right)
    {
        return !(left == right);
    }

    public static bool operator >(RabbitMqBatchContainer? left, RabbitMqBatchContainer? right)
    {
        return Compare(left, right) > 0;
    }

    public static bool operator >=(RabbitMqBatchContainer? left, RabbitMqBatchContainer? right)
    {
        return Compare(left, right) >= 0;
    }

    public static bool operator <(RabbitMqBatchContainer? left, RabbitMqBatchContainer? right)
    {
        return Compare(left, right) < 0;
    }

    public static bool operator <=(RabbitMqBatchContainer? left, RabbitMqBatchContainer? right)
    {
        return Compare(left, right) <= 0;
    }

    /// <inheritdoc />
    public IEnumerable<Tuple<T, StreamSequenceToken?>> GetEvents<T>()
    {
        var events = _events.OfType<T>();
        var returns = events.Select((@event, i) =>
            Tuple.Create<T, StreamSequenceToken?>(
                @event,
                new RabbitMqStreamSequenceToken(Convert.ToInt64(_deliveryTag ?? 0), i)));
        return returns;
    }

    /// <inheritdoc />
    public bool ImportRequestContext()
    {
        if (_requestContext is null)
        {
            return false;
        }

        RequestContextExtensions.Import(_requestContext);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(RabbitMqBatchContainer? other)
    {
        return Compare(this, other);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not RabbitMqBatchContainer other)
        {
            return false;
        }

        return CompareTo(other) == 0;
    }

    /// <summary>
    /// Get Hash Code.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return 397 * _events.GetHashCode() ^ (_requestContext?.GetHashCode() ?? 17) ^ DeliveryTag.GetHashCode();
    }

    /// <summary>
    /// Update Delivery Tag.
    /// </summary>
    /// <param name="deliveryTag">The delivery tag.</param>
    internal void UpdateDeliveryTag(ulong deliveryTag)
    {
        _deliveryTag = deliveryTag;
        _sequenceToken = new RabbitMqStreamSequenceToken(deliveryTag);
    }

    private static int Compare(RabbitMqBatchContainer? left, RabbitMqBatchContainer? right)
    {
        switch (left)
        {
            case null when right is null:
                return 0;
            case null:
                return -1;
            default:
            {
                if (right is null)
                {
                    return 1;
                }

                break;
            }
        }

        if (left.DeliveryTag.Equals(right.DeliveryTag))
        {
            return 0;
        }

        return left.DeliveryTag > right.DeliveryTag ? 1 : -1;
    }
}
