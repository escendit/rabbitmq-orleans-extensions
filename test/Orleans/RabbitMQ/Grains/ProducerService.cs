// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Tests.Grains;

using global::Orleans.EventSourcing;
using global::Orleans.Streams;
using Microsoft.Extensions.Logging;

/// <summary>
/// Producer Service.
/// </summary>
public class ProducerService : JournaledGrain<ProducerState, ProducerEvent>, IProducerService
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProducerService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public ProducerService(ILogger<ProducerService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task CallAsync(int newValue, GrainCancellationToken? cancellationToken = default)
    {
        _logger.LogDebug("Call");
        var streamProvider = this.GetStreamProvider("silo");
        var stream = streamProvider.GetStream<ProducerEvent>("ProducerEvent", Guid.Empty);

        var @event = new ProducerEvent
        {
            NewValue = newValue,
        };

        RaiseEvent(@event);
        await ConfirmEvents();
        await stream.OnNextAsync(@event);
    }

    /// <inheritdoc />
    public Task<int> GetAsync(GrainCancellationToken? cancellationToken = default)
    {
        _logger.LogDebug("Get");
        return Task.FromResult(State.Value);
    }
}
