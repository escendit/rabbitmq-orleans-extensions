// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace RabbitMQ.Configuration;

using Microsoft.Extensions.Logging;
using Orleans.TestingHost;

/// <summary>
/// Test Silo Configurator.
/// </summary>
public class TestSiloConfigurator : ISiloConfigurator
{
    /// <inheritdoc />
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder
            .ConfigureLogging(options =>
            {
                options.AddConsole();
                options.AddFilter("Escendit", LogLevel.Debug);
                options.AddFilter("Default", LogLevel.Information);
                options.AddFilter("Orleans", LogLevel.Error);
                options.AddFilter("Orleans.Streams", LogLevel.Information);
                options.AddFilter("RabbitMQ", LogLevel.Information);
                options.SetMinimumLevel(LogLevel.Debug);
            });
        siloBuilder
            .AddMemoryGrainStorageAsDefault()
            .AddLogStorageBasedLogConsistencyProviderAsDefault();
    }
}
