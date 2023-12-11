// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Builder;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Escendit.Orleans.Streaming.RabbitMQ.Tests Builder.
/// </summary>
public interface IRabbitMqBuilder
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    string Name { get; }

    /// <summary>
    /// Finish builder.
    /// </summary>
    /// <returns>The updated service collection.</returns>
    IServiceCollection Build();
}
