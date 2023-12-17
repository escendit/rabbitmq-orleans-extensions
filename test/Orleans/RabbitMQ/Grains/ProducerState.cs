// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Tests.Grains;

/// <summary>
/// Producer State.
/// </summary>
[GenerateSerializer]
[Alias("producerState")]
public class ProducerState
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    [Id(0)]
    public int Value { get; set; }

    /// <summary>
    /// Apply Producer Event.
    /// </summary>
    /// <param name="event">The event.</param>
    public void Apply(ProducerEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);
        Value = @event.NewValue;
    }
}
