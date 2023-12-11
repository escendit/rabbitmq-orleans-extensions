// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Core;

using System.Text.Json.Serialization;
using global::Orleans.Runtime;
using global::Orleans.Streams;

/// <summary>
/// Escendit.Orleans.Streaming.RabbitMQ.Tests Batch Container.
/// </summary>
internal class RabbitMqBatchContainer : IBatchContainer, IComparable<RabbitMqBatchContainer>
{
    [Id(0)]
    [JsonPropertyName("events")]
    private readonly ICollection<object> _events;

    [Id(1)]
    [JsonPropertyName("requestContext")]
    private readonly Dictionary<string, object> _requestContext;

    [Id(2)]
    [JsonPropertyName("sequenceToken")]
    private readonly RabbitMqStreamSequenceToken _sequenceToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqBatchContainer"/> class.
    /// </summary>
    /// <param name="streamId">The stream id.</param>
    /// <param name="events">The events.</param>
    /// <param name="requestContext">The request context.</param>
    /// <param name="sequenceToken">The sequence token.</param>
    [JsonConstructor]
    public RabbitMqBatchContainer(
        StreamId streamId,
        ICollection<object> events,
        Dictionary<string, object> requestContext,
        RabbitMqStreamSequenceToken sequenceToken)
    {
        StreamId = streamId;
        _events = events;
        _requestContext = requestContext;
        _sequenceToken = sequenceToken;
    }

    /// <inheritdoc />
    public StreamSequenceToken SequenceToken => _sequenceToken;

    /// <inheritdoc />
    [Id(3)]
    [JsonPropertyName("streamId")]
    public StreamId StreamId { get; }

    public static bool operator ==(RabbitMqBatchContainer? left, RabbitMqBatchContainer? right)
    {
        return left?.Equals(right) ?? ReferenceEquals(right, null);
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
        return Compare(left, right) > 0;
    }

    public static bool operator <(RabbitMqBatchContainer? left, RabbitMqBatchContainer? right)
    {
        return Compare(left, right) < 0;
    }

    public static bool operator <=(RabbitMqBatchContainer? left, RabbitMqBatchContainer? right)
    {
        return Compare(left, right) > 0;
    }

    /// <inheritdoc />
    public IEnumerable<Tuple<T, StreamSequenceToken>> GetEvents<T>()
    {
        return _events.OfType<T>().Select((e, i) => Tuple.Create<T, StreamSequenceToken>(e, new RabbitMqStreamSequenceToken(_sequenceToken.SequenceNumber, i)));
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
        return other?.CompareTo(this) ?? 0;
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
        return 397 * _events.GetHashCode() ^ (_requestContext?.GetHashCode() ?? 17) ^ _sequenceToken.GetHashCode();
    }

    /// <summary>
    /// Update Sequence Token.
    /// </summary>
    /// <param name="streamSequenceToken">The stream sequence token.</param>
    internal void UpdateSequenceToken(RabbitMqStreamSequenceToken streamSequenceToken)
    {
        _sequenceToken.Update(streamSequenceToken.SequenceNumber, streamSequenceToken.EventIndex);
    }

    private static int Compare(RabbitMqBatchContainer? left, RabbitMqBatchContainer? right)
    {
        if (left == right)
        {
            return 0;
        }

        return left > right ? 1 : -1;
    }
}
