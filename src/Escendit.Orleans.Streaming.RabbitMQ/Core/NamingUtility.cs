// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Core;

using global::Orleans.Configuration;
using global::Orleans.Streams;

/// <summary>
/// Naming Utility.
/// </summary>
internal static class NamingUtility
{
    /// <summary>
    /// Create a Name for a Queue.
    /// </summary>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="queueId">The queue id.</param>
    /// <returns>The unique name for a queue or an exchange.</returns>
    public static string CreateNameForQueue(ClusterOptions clusterOptions, QueueId queueId)
    {
        return $"queue-{clusterOptions.ClusterId}-{clusterOptions.ServiceId}-{queueId.ToString()}";
    }

    /// <summary>
    /// Create a Name for a Stream.
    /// </summary>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="queueId">The queue id.</param>
    /// <returns>The unique name for a stream (queue).</returns>
    public static string CreateNameForStream(ClusterOptions clusterOptions, QueueId queueId)
    {
        return $"stream-{clusterOptions.ClusterId}-{clusterOptions.ServiceId}-{queueId.ToString()}";
    }
}
