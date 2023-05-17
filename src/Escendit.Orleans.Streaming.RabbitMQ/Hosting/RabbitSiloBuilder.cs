// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

using Microsoft.Extensions.DependencyInjection;

/// <inheritdoc />
public class RabbitSiloBuilder : ISiloBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitSiloBuilder"/> class.
    /// </summary>
    /// <param name="siloBuilder">The silo builder.</param>
    /// <param name="name">The name.</param>
    internal RabbitSiloBuilder(ISiloBuilder siloBuilder, string name)
    {
        ArgumentNullException.ThrowIfNull(siloBuilder);
        Services = siloBuilder.Services;
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
