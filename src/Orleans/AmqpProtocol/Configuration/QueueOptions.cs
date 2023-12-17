// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Configuration;

using global::RabbitMQ.Client;
using RabbitMQ.Configuration;

/// <summary>
/// Escendit.Orleans.Streaming.RabbitMQ.Tests AMQP Queue Options.
/// </summary>
public class QueueOptions : OptionsBase
{
    private const string DefaultExchangeKey = "queue";

    /// <summary>
    /// Gets or sets the exchange type.
    /// </summary>
    /// <para>The default is <see cref="ExchangeType.Direct"/>.</para>
    /// <value>The exchange type.</value>
    public string Type { get; set; } = ExchangeType.Topic;

    /// <summary>
    /// Gets or sets the exchange name.
    /// </summary>
    /// <value>The exchange name.</value>
    public string Name { get; set; } = DefaultExchangeKey;

    /// <summary>
    /// Gets or sets a value indicating whether the queue is durable.
    /// </summary>
    /// <value>The flag if the queue is durable.</value>
    public bool IsDurable { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the queue is exclusive.
    /// </summary>
    /// <value>The flag if the queue is exclusive.</value>
    public bool IsExclusive { get; set; }
}
