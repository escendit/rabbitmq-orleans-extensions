// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace RabbitMQ.Tests.Extensions;

using Escendit.Extensions.DependencyInjection.RabbitMQ.Abstractions;

/// <summary>
/// Connection Extensions.
/// </summary>
public static class ConnectionExtensions
{
    /// <summary>
    /// Setup.
    /// </summary>
    /// <param name="options">The connection options.</param>
    public static void Setup(ConnectionOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.UserName = "guest";
        options.Password = "guest";
        options.VirtualHost = "/";
        options.Endpoints.Add(new Endpoint
        {
            HostName = "localhost",
        });
    }
}
