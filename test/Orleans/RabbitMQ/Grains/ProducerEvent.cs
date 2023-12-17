// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Tests.Grains;

/// <summary>
/// Producer Event.
/// </summary>
[GenerateSerializer]
[Alias("producerEvent")]
public class ProducerEvent
{
    /// <summary>
    /// Gets the new value.
    /// </summary>
    /// <value>The new value.</value>
    [Id(0)]
    public int NewValue { get; init; }
}
