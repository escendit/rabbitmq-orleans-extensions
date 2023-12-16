// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.Tests.Collections;

using Fixtures;

/// <summary>
/// Host Collection Fixture.
/// </summary>
[CollectionDefinition(Name)]
public class HostCollectionFixture : ICollectionFixture<HostBuilderFixture>
{
    /// <summary>
    /// Name.
    /// </summary>
    public const string Name = "HostBuilderCollection";
}
