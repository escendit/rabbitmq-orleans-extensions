// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Tests.Grains;

/// <summary>
/// Test Producer Service.
/// </summary>
public interface IProducerService : IGrainWithGuidKey
{
    /// <summary>
    /// Call.
    /// </summary>
    /// <param name="newValue">The new value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The void.</returns>
    Task CallAsync(int newValue, GrainCancellationToken? cancellationToken = default);

    /// <summary>
    /// Get.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The value.</returns>
    Task<int> GetAsync(GrainCancellationToken? cancellationToken = default);
}
