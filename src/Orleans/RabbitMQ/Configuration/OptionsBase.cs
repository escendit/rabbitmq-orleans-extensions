﻿// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Configuration;

using System.ComponentModel;
using global::Orleans.Streams;

/// <summary>
/// Options Base.
/// </summary>
public abstract class OptionsBase
{
    /// <summary>
    /// Gets the stream failure handler.
    /// </summary>
    /// <value>The stream failure handler.</value>
    [Browsable(false)]
    public Func<QueueId, Task<IStreamFailureHandler>> StreamFailureHandler { get; internal set; } = _ =>
        Task.FromResult<IStreamFailureHandler>(new NoOpStreamDeliveryFailureHandler());
}
