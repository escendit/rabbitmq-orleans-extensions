// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.AmqpProtocol.Configuration;

using global::Orleans.Streams;
using Handlers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <inheritdoc />
internal class ConfigureQueueOptions : IConfigureOptions<QueueOptions>
{
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureQueueOptions"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger.</param>
    public ConfigureQueueOptions(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public void Configure(QueueOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.StreamFailureHandler = (queueId) =>
            Task.FromResult<IStreamFailureHandler>(
                new RabbitMqStreamDeliveryFailureHandler(_loggerFactory.CreateLogger<RabbitMqStreamDeliveryFailureHandler>(), queueId, options.ShouldFaultSubscriptionOnError));
    }
}
