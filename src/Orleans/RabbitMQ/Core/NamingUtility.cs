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
    private const string Stream = "stream";
    private const string Queue = "queue";

    /// <summary>
    /// Create a Name for a Queue or an Exchange.
    /// </summary>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="queueId">The queue id.</param>
    /// <returns>The unique name for a queue or an exchange.</returns>
    public static string CreateNameForQueue(ClusterOptions clusterOptions, QueueId queueId)
    {
        return CreateNameInternal(Queue, clusterOptions.ClusterId, clusterOptions.ServiceId, queueId.ToString());
    }

    /// <summary>
    /// Create a Name for a Queue or an Exchange.
    /// </summary>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="name">The exchange name.</param>
    /// <returns>The unique name for an exchange.</returns>
    public static string CreateNameForQueue(ClusterOptions clusterOptions, string name)
    {
        return CreateNameInternal(Queue, clusterOptions.ClusterId, clusterOptions.ServiceId, name);
    }

    /// <summary>
    /// Create a Name for a Stream.
    /// </summary>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="queueId">The queue id.</param>
    /// <returns>The unique name for a stream (queue).</returns>
    public static string CreateNameForStream(ClusterOptions clusterOptions, QueueId queueId)
    {
        return CreateNameInternal(Stream, clusterOptions.ClusterId, clusterOptions.ServiceId, queueId.ToString());
    }

    private static string CreateNameInternal(string type, string clusterId, string serviceId, string name)
    {
        return $"{type}-{clusterId}-{serviceId}-{name}";
    }
}
