// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Options;

using System.Diagnostics.CodeAnalysis;
using global::RabbitMQ.Client;

/// <summary>
/// Rabbit MQ Stream Provider Options.
/// </summary>
[DynamicallyAccessedMembers(
    DynamicallyAccessedMemberTypes.PublicProperties)]
public record RabbitQueueOptions : RabbitOptionsBase
{
    /// <summary>
    /// Default Exchange Key.
    /// </summary>
    private const string DefaultExchangekey = "exchange";

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
    public string Name { get; set; } = DefaultExchangekey;

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
    /// Gets the default Port.
    /// </summary>
    /// <value>The default port.</value>
    public int DefaultPort
    {
        get
        {
            if (SslOptions is null)
            {
                return 5672;
            }

            return SslOptions.Enabled ? 5671 : 5672;
        }
    }
}
