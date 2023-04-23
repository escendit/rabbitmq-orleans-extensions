// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Options;

/// <summary>
/// Rabbit Endpoint.
/// </summary>
public class RabbitEndpoint
{
    /// <summary>
    /// Gets or sets the host name.
    /// </summary>
    /// <value>The host name.</value>
    public string HostName { get; set; } = default!;

    /// <summary>
    /// Gets or sets the port.
    /// </summary>
    /// <value>The port.</value>
    public int? Port { get; set; }
}
