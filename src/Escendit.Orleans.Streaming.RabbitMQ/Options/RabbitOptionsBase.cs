// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Options;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using global::Orleans.Configuration;
using global::Orleans.Streams;

/// <summary>
/// Shared RabbitMQ Options.
/// </summary>
[DynamicallyAccessedMembers(
    DynamicallyAccessedMemberTypes.All)]
public record RabbitOptionsBase
{
    /// <summary>
    /// Section Name.
    /// </summary>
    public const string SectionName = "Escendit.Orleans.Streaming.RabbitMQ";

    /// <summary>
    /// Gets the endpoints.
    /// </summary>
    /// <value>The endpoints.</value>
    public IList<RabbitEndpoint> Endpoints { get; init; } = new List<RabbitEndpoint>();

    /// <summary>
    /// Gets or sets the virtual host.
    /// </summary>
    /// <value>The virtual host.</value>
    public string? VirtualHost { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    /// <value>The username.</value>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>The password.</value>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the heartbeat.
    /// </summary>
    /// <value>The heartbeat.</value>
    public TimeSpan Heartbeat { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets or sets the ssl options.
    /// </summary>
    /// <value>The ssl options.</value>
    public RabbitSslOptions? SslOptions { get; set; }

    /// <summary>
    /// Gets or sets the client provided name.
    /// </summary>
    /// <value>The client provided name.</value>
    public string? ClientProvidedName { get; set; } = SectionName;

    /// <summary>
    /// Gets or sets the client provided name. Default is defined here <see cref="HashRingStreamQueueMapperOptions.DEFAULT_NUM_QUEUES"/>.
    /// </summary>
    /// <value>The total queue count.</value>
    public int TotalQueueCount { get; set; } = HashRingStreamQueueMapperOptions.DEFAULT_NUM_QUEUES;

    /// <summary>
    /// Gets or sets the cache size. Default is defined here: <see cref="SimpleQueueCacheOptions.DEFAULT_CACHE_SIZE"/>.
    /// </summary>
    /// <value>The cache size.</value>
    public int CacheSize { get; set; } = SimpleQueueCacheOptions.DEFAULT_CACHE_SIZE;

    /// <summary>
    /// Sets the stream failure handler factory.
    /// </summary>
    /// <value>The stream failure handler factory.</value>
    [Browsable(false)]
    public Func<QueueId, Task<IStreamFailureHandler>> StreamFailureHandlerFactory { internal get; set; } = _ => Task.FromResult<IStreamFailureHandler>(new NoOpStreamDeliveryFailureHandler());
}
