// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.Tests;

using Collections;
using Fixtures;
using global::Orleans.TestingHost;
using Xunit.Categories;

/// <summary>
/// Client Builder Tests.
/// </summary>
[Collection(ClusterCollectionFixture.Name)]
public class ClientBuilderTests
{
    private readonly TestCluster _cluster;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientBuilderTests"/> class.
    /// </summary>
    /// <param name="fixture">The cluster fixture.</param>
    public ClientBuilderTests(ClusterFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _cluster = fixture.Cluster;
    }

    /// <summary>
    /// Start.
    /// </summary>
    [Fact]
    [IntegrationTest]
    public void ClusterIsUp()
    {
        Assert.NotNull(_cluster);
    }
}
