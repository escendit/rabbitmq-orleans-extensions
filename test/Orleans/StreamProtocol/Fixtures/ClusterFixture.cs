// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.Tests.Fixtures;

using System.Diagnostics.CodeAnalysis;
using Configuration;
using global::Orleans.TestingHost;
using global::RabbitMQ.Configuration;

/// <summary>
/// Cluster Fixture.
/// </summary>
[DynamicallyAccessedMembers(
    DynamicallyAccessedMemberTypes.PublicConstructors)]
public sealed class ClusterFixture : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClusterFixture"/> class.
    /// </summary>
    public ClusterFixture()
    {
        var builder = new TestClusterBuilder()
        {
            Options =
            {
                ClusterId = "testCluster",
                ServiceId = "testService",
            },
        };
        builder
            .AddSiloBuilderConfigurator<TestSiloConfigurator>()
            .AddClientBuilderConfigurator<RabbitMqClientConfigurator>()
            .AddSiloBuilderConfigurator<RabbitMqSiloConfigurator>();
        Cluster = builder.Build();
        Cluster.Deploy();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="ClusterFixture"/> class.
    /// </summary>
    ~ClusterFixture()
    {
        Dispose(false);
    }

    /// <summary>
    /// Gets the cluster.
    /// </summary>
    /// <value>The cluster.</value>
    public TestCluster Cluster { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// The dispose pattern.
    /// </summary>
    /// <param name="disposing">if cleanup.</param>
    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            Cluster.StopAllSilos();
        }
    }
}
