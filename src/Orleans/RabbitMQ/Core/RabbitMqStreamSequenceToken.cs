// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Core;

using System.Globalization;
using System.Text.Json.Serialization;
using global::Orleans.Streams;

/// <summary>
/// Escendit.Orleans.Streaming.RabbitMQ.Tests Stream Sequence Token.
/// </summary>
[GenerateSerializer]
[Serializable]
[Alias("rabbitmq-stream-sequence-token")]
public class RabbitMqStreamSequenceToken : StreamSequenceToken
{
    [Id(0)]
    [JsonPropertyName("sequenceNumber")]
    private long _sequenceNumber;

    [Id(1)]
    [JsonPropertyName("eventIndex")]
    private int _eventIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqStreamSequenceToken"/> class.
    /// </summary>
    /// <param name="sequenceNumber">The sequence number.</param>
    /// <param name="eventIndex">The event index.</param>
    [JsonConstructor]
    public RabbitMqStreamSequenceToken(long sequenceNumber, int eventIndex)
    {
        _sequenceNumber = sequenceNumber;
        _eventIndex = eventIndex;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqStreamSequenceToken"/> class.
    /// </summary>
    /// <param name="sequenceToken">The sequence token.</param>
    public RabbitMqStreamSequenceToken(StreamSequenceToken sequenceToken)
    {
        ArgumentNullException.ThrowIfNull(sequenceToken);
        _sequenceNumber = sequenceToken.SequenceNumber;
        _eventIndex = sequenceToken.EventIndex;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqStreamSequenceToken"/> class.
    /// </summary>
    /// <param name="sequenceNumber">The sequence number.</param>
    public RabbitMqStreamSequenceToken(ulong sequenceNumber)
    {
        _sequenceNumber = Convert.ToInt64(sequenceNumber);
        _eventIndex = 0;
    }

    /// <summary>
    /// Gets or sets the sequence number.
    /// </summary>
    /// <value>The sequence number.</value>
    [JsonIgnore]
    public override long SequenceNumber
    {
        get => _sequenceNumber;
        protected set => _sequenceNumber = value;
    }

    /// <summary>
    /// Gets or sets the event index.
    /// </summary>
    /// <value>The event index.</value>
    [JsonIgnore]
    public override int EventIndex
    {
        get => _eventIndex;
        protected set => _eventIndex = value;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return Equals(obj as RabbitMqStreamSequenceToken);
    }

    /// <inheritdoc />
    public override bool Equals(StreamSequenceToken? other)
    {
        var token = other as RabbitMqStreamSequenceToken;
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

        if (other is not RabbitMqStreamSequenceToken token)
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
            "[RabbitMQStreamSequenceToken: Num: {0}, Index: {1}]",
            SequenceNumber.ToString(CultureInfo.InvariantCulture),
            EventIndex.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Update Sequence Token.
    /// </summary>
    /// <param name="sequenceNumber">The sequence token.</param>
    /// <param name="eventIndex">The event index.</param>
    internal void Update(long sequenceNumber, int eventIndex)
    {
        _sequenceNumber = sequenceNumber;
        _eventIndex = eventIndex;
    }
}
