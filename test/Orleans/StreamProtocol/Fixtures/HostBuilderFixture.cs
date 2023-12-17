// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.Tests.Fixtures;

using Microsoft.Extensions.Hosting;

/// <summary>
/// Host Builder Fixture.
/// </summary>
public sealed class HostBuilderFixture
{
    private readonly string _pubSubStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostBuilderFixture"/> class.
    /// </summary>
    public HostBuilderFixture()
    {
        _pubSubStore = "PubSubStore";
    }

    /// <summary>
    /// Create Host Builder.
    /// </summary>
    /// <param name="siloBuilder">The silo builder.</param>
    /// <returns>The new host builder.</returns>
    public IHostBuilder CreateSiloHostBuilder(Action<HostBuilderContext, ISiloBuilder> siloBuilder)
    {
        return Host
            .CreateDefaultBuilder()
            .UseOrleans(SetupSiloDefaults)
            .UseOrleans(siloBuilder);
    }

    /// <summary>
    /// Create Host Builder.
    /// </summary>
    /// <param name="siloBuilder">The silo builder.</param>
    /// <returns>The host builder.</returns>
    public IHostBuilder CreateSiloHostBuilder(Action<ISiloBuilder> siloBuilder)
    {
        return Host
            .CreateDefaultBuilder()
            .UseOrleans(SetupSiloDefaults)
            .UseOrleans(siloBuilder);
    }

    /// <summary>
    /// Create Cluster Client Host Builder.
    /// </summary>
    /// <param name="clientBuilder">The client builder.</param>
    /// <returns>The host builder.</returns>
    public IHostBuilder CreateClusterClientHostBuilder(Action<HostBuilderContext, IClientBuilder> clientBuilder)
    {
        return Host
            .CreateDefaultBuilder()
            .UseOrleansClient(SetupClientDefaults)
            .UseOrleansClient(clientBuilder);
    }

    /// <summary>
    /// Create Cluster Client Host Builder.
    /// </summary>
    /// <param name="clientBuilder">The client builder.</param>
    /// <returns>The host builder.</returns>
    public IHostBuilder CreateClusterClientHostBuilder(Action<IClientBuilder> clientBuilder)
    {
        return Host
            .CreateDefaultBuilder()
            .UseOrleansClient(SetupClientDefaults)
            .UseOrleansClient(clientBuilder);
    }

    private void SetupSiloDefaults(HostBuilderContext context, ISiloBuilder siloBuilder)
    {
        siloBuilder
            .UseLocalhostClustering()
            .AddMemoryGrainStorage(_pubSubStore)
            .AddMemoryGrainStorageAsDefault();
    }

    private void SetupClientDefaults(IClientBuilder clientBuilder)
    {
        clientBuilder
            .AddMemoryStreams(_pubSubStore)
            .UseLocalhostClustering();
    }
}
