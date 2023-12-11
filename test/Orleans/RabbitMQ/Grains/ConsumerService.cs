// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Tests.Grains;

using global::Orleans.EventSourcing;
using global::Orleans.Runtime;
using global::Orleans.Streams;

/// <summary>
/// Consumer Service.
/// </summary>
[ImplicitStreamSubscription("ProducerEvent")]
public class ConsumerService : JournaledGrain<ConsumerState, ConsumerEvent>, IConsumerService, IAsyncObserver<ProducerEvent>
{
    private StreamSubscriptionHandle<ProducerEvent>? _streamSubscriptionHandle;

    /// <inheritdoc />
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider("silo");
        var stream = streamProvider.GetStream<ProducerEvent>("ProducerEvent", Guid.Empty);

        if (_streamSubscriptionHandle is null)
        {
            _streamSubscriptionHandle = await stream.SubscribeAsync(OnNextAsync);
        }
        else
        {
            await _streamSubscriptionHandle.ResumeAsync(OnNextAsync);
        }

        await base.OnActivateAsync(cancellationToken);
    }

    /// <inheritdoc />
    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        if (_streamSubscriptionHandle is not null)
        {
            await _streamSubscriptionHandle!.UnsubscribeAsync();
        }

        await base.OnDeactivateAsync(reason, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetAsync(GrainCancellationToken? cancellationToken = default)
    {
        return Task.FromResult(State.Value);
    }

    /// <inheritdoc />
    public async Task OnNextAsync(ProducerEvent item, StreamSequenceToken? token = null)
    {
        var listenedEvent = new ConsumerEvent
        {
            Value = item.NewValue,
        };

        RaiseEvent(listenedEvent);
        await ConfirmEvents();
    }

    /// <inheritdoc />
    public Task OnCompletedAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task OnErrorAsync(Exception ex)
    {
        throw new NotImplementedException();
    }
}
