// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Options;

using global::RabbitMQ.Client;

/// <summary>
/// Rabbit MQ Stream Provider Options.
/// </summary>
public record RabbitQueueOptions : RabbitOptionsBase
{
    /// <summary>
    /// Default Routing Key.
    /// </summary>
    private const string DefaultRoutingKey = "queue";

    /// <summary>
    /// Gets or sets the exchange type.
    /// </summary>
    /// <para>The default is <see cref="ExchangeType.Direct"/>.</para>
    /// <value>The exchange type.</value>
    public string QueueType { get; set; } = ExchangeType.Direct;

    /// <summary>
    /// Gets or sets the routing key.
    /// </summary>
    /// <value>The routing key.</value>
    public string RoutingKey { get; set; } = DefaultRoutingKey;

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

    /// <summary>
    /// Get Default Port.
    /// </summary>
    /// <returns>The port.</returns>
    public int GetDefaultPort()
    {
        if (SslOptions is null)
        {
            return 5672;
        }

        return SslOptions.Enabled ? 5671 : 5672;
    }
}
