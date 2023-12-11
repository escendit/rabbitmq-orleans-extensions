// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Builder;

/// <summary>
/// Escendit.Orleans.Streaming.RabbitMQ.Tests Client Options Builder.
/// </summary>
public interface IRabbitMqClientOptionsBuilder : IRabbitMqBuilder, IClientBuilder
{
    /// <summary>
    /// Gets the configurator.
    /// </summary>
    /// <value>The configurator.</value>
    internal ClusterClientPersistentStreamConfigurator Configurator { get; }
}
