// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Queue;

using global::Orleans.Runtime;
using global::Orleans.Streams;

/// <summary>
/// Default Queue Adapter.
/// </summary>
public class DefaultQueueAdapter : IQueueAdapter
{
    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc />
    public bool IsRewindable { get; }

    /// <inheritdoc />
    public StreamProviderDirection Direction { get; }

    /// <inheritdoc />
    public Task QueueMessageBatchAsync<T>(
        StreamId streamId,
        IEnumerable<T> events,
        StreamSequenceToken token,
        Dictionary<string, object> requestContext)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
    {
        throw new NotImplementedException();
    }
}
