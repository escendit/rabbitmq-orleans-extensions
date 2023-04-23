// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Tests.Grains;

/// <summary>
/// Consumer Service.
/// </summary>
public interface IConsumerService : IGrainWithGuidKey
{
    /// <summary>
    /// Get.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<int> GetAsync(GrainCancellationToken? cancellationToken = default);
}
