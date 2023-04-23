// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Tests.Grains;

/// <summary>
/// Consumer State.
/// </summary>
public class ConsumerState
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    public int Value { get; set; }

    /// <summary>
    /// Apply Consumer Event.
    /// </summary>
    /// <param name="event">The event.</param>
    public void Apply(ConsumerEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);
        Value = @event.Value;
    }
}
