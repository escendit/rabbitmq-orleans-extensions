// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Tests.Grains;

/// <summary>
/// Consumer Event.
/// </summary>
public class ConsumerEvent
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>The value.</value>
    public int Value { get; init; }
}
