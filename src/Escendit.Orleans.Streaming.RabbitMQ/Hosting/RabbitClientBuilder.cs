// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Microsoft.Extensions.DependencyInjection;

/// <inheritdoc />
public class RabbitClientBuilder : IClientBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitClientBuilder"/> class.
    /// </summary>
    /// <param name="clientBuilder">The client builder.</param>
    /// <param name="name">The name.</param>
    internal RabbitClientBuilder(
        IClientBuilder clientBuilder,
        string name)
    {
        ArgumentNullException.ThrowIfNull(clientBuilder);
        Services = clientBuilder.Services;
        Name = name;
    }

    /// <inheritdoc />
    public IServiceCollection Services { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; }
}
