// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace RabbitMQ.Tests.Generators;

using System.Collections;
using Orleans.Streams;

/// <summary>
/// Stream PubSub Type Generator.
/// </summary>
public class StreamPubSubTypeGenerator : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new List<object[]>()
    {
        new object[] { StreamPubSubType.ExplicitGrainBasedAndImplicit },
        new object[] { StreamPubSubType.ImplicitOnly },
        new object[] { StreamPubSubType.ExplicitGrainBasedOnly },
    };

    /// <inheritdoc/>
    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
