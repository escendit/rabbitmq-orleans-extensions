// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Core;

using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using global::Orleans;
using global::Orleans.Runtime;
using global::Orleans.Streams;

/// <summary>
/// Rabbit MQ Batch Container.
/// </summary>
[Serializable]
[GenerateSerializer]
public class RabbitBatchContainer : IBatchContainer, IComparable<RabbitBatchContainer>
{
    [Id(0)]
    [JsonPropertyName("sequenceToken")]
    private RabbitStreamSequenceToken _sequenceToken;

    [Id(1)]
    [JsonPropertyName("events")]
    private ICollection<object> _events;

    [Id(2)]
    [JsonPropertyName("requestContext")]
    private Dictionary<string, object>? _requestContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitBatchContainer"/> class.
    /// </summary>
    /// <param name="streamId">The stream id.</param>
    /// <param name="events">The events.</param>
    /// <param name="requestContext">The request context.</param>
    /// <param name="sequenceToken">The sequence token.</param>
    [JsonConstructor]
    public RabbitBatchContainer(
        StreamId streamId,
        ICollection<object> events,
        Dictionary<string, object> requestContext,
        RabbitStreamSequenceToken sequenceToken)
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

    /// <inheritdoc />
    public IEnumerable<Tuple<T, StreamSequenceToken>> GetEvents<T>()
    {
        return _events.OfType<T>().Select((e, i) => Tuple.Create<T, StreamSequenceToken>(e, new RabbitStreamSequenceToken(_sequenceToken.SequenceNumber, i)));
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
    public int CompareTo(RabbitBatchContainer? other)
    {
        return other?.CompareTo(this) ?? 0;
    }

    /// <summary>
    /// Update Sequence Token.
    /// </summary>
    /// <param name="streamSequenceToken">The stream sequence token.</param>
    internal void UpdateSequenceToken(RabbitStreamSequenceToken streamSequenceToken)
    {
        _sequenceToken =
            new RabbitStreamSequenceToken(
                streamSequenceToken.SequenceNumber,
                streamSequenceToken.EventIndex);
    }
}
