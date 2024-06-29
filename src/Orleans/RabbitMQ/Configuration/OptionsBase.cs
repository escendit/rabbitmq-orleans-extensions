// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Configuration;

using System.ComponentModel;
using global::Orleans.Streams;

/// <summary>
/// Options Base.
/// </summary>
public class OptionsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OptionsBase"/> class.
    /// </summary>
    protected OptionsBase()
    {
    }

    /// <summary>
    /// Gets or sets the stream failure handler.
    /// </summary>
    /// <value>The stream failure handler.</value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Func<QueueId, Task<IStreamFailureHandler>> StreamFailureHandler { get; set; } = _ =>
        Task.FromResult<IStreamFailureHandler>(new NoOpStreamDeliveryFailureHandler());

    /// <summary>
    /// Gets or sets a value indicating whether it should fault on subscription error.
    /// </summary>
    /// <value>The flag if subscription should fault.</value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool ShouldFaultSubscriptionOnError { get; set; }
}
