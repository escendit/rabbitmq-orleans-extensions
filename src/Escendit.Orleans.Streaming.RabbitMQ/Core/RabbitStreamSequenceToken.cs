// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Core;

using System.Globalization;
using System.Text.Json.Serialization;
using global::Orleans.Streams;

/// <summary>
/// Rabbit MQ Stream Sequence Token.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class RabbitStreamSequenceToken : StreamSequenceToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitStreamSequenceToken"/> class.
    /// </summary>
    /// <param name="sequenceNumber">The sequence number.</param>
    /// <param name="eventIndex">The event index.</param>
    public RabbitStreamSequenceToken(long sequenceNumber, int eventIndex = 0)
    {
        SequenceNumber = sequenceNumber;
        EventIndex = eventIndex;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitStreamSequenceToken"/> class.
    /// </summary>
    public RabbitStreamSequenceToken()
    {
    }

    /// <summary>
    /// Gets or sets the sequence number.
    /// </summary>
    /// <value>The sequence number.</value>
    [Id(0)]
    [JsonPropertyName("sequenceNumber")]
    public override long SequenceNumber { get; protected set; }

    /// <summary>
    /// Gets or sets the event index.
    /// </summary>
    /// <value>The event index.</value>
    [Id(1)]
    [JsonPropertyName("eventIndex")]
    public override int EventIndex { get; protected set; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return Equals(obj as RabbitStreamSequenceToken);
    }

    /// <inheritdoc />
    public override bool Equals(StreamSequenceToken? other)
    {
        var token = other as RabbitStreamSequenceToken;
        return token is not null
               && token.SequenceNumber == SequenceNumber
               && token.EventIndex == EventIndex;
    }

    /// <inheritdoc />
    public override int CompareTo(StreamSequenceToken? other)
    {
        if (other is null)
        {
            return 1;
        }

        if (other is not RabbitStreamSequenceToken token)
        {
            throw new ArgumentOutOfRangeException(nameof(other));
        }

        var difference = SequenceNumber.CompareTo(token.SequenceNumber);
        return difference != 0 ? difference : EventIndex.CompareTo(token.EventIndex);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return (EventIndex * 397) ^ SequenceNumber.GetHashCode();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "[RabbitMQStreamSequenceToken: Num: {0}, Index: {1}",
            SequenceNumber.ToString(CultureInfo.InvariantCulture),
            EventIndex.ToString(CultureInfo.InvariantCulture));
    }
}
