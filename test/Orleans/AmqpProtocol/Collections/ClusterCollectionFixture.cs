// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Tests.Collections;

using Fixtures;

/// <summary>
/// Cluster Collection.
/// </summary>
[CollectionDefinition(Name)]
public class ClusterCollectionFixture : ICollectionFixture<ClusterFixture>
{
    /// <summary>
    /// Cluster Collection Name.
    /// </summary>
    public const string Name = "ClusterCollection";
}
