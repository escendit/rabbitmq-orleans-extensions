// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Core;

using global::Orleans.Streams;

/// <summary>
/// Naming Utility.
/// </summary>
internal class NamingUtility
{
    /// <summary>
    /// Create a Name for a Queue.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="queueId">The queue id.</param>
    /// <returns>The unique name for a queue or an exchange.</returns>
    public static string CreateNameForQueue(string name, QueueId queueId)
    {
        return $"queue-{name}-{queueId.ToString()}";
    }

    /// <summary>
    /// Create a Name for a Stream.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="queueId">The queue id.</param>
    /// <returns>The unique name for a stream (queue).</returns>
    public static string CreateNameForStream(string name, QueueId queueId)
    {
        return $"stream-{name}-{queueId.ToString()}";
    }
}
