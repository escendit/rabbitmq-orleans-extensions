// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Queue;

using global::Orleans.Streams;
using global::RabbitMQ.Client;
using Microsoft.Extensions.Options;

/// <summary>
/// Default Queue Adapter Factory.
/// </summary>
internal class DefaultQueueAdapterFactory : IQueueAdapterFactory
{
    private IConnectionFactory _connectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultQueueAdapterFactory"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="options">The options.</param>
    public DefaultQueueAdapterFactory(string name, IOptionsMonitor<object> options)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(options);
        options.OnChange(UpdateOptions);
        UpdateOptions(options.CurrentValue, name);

        _connectionFactory = new ConnectionFactory()
        {
        };
    }

    /// <inheritdoc />
    public Task<IQueueAdapter> CreateAdapter()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IQueueAdapterCache GetQueueAdapterCache()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IStreamQueueMapper GetStreamQueueMapper()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<IStreamFailureHandler> GetDeliveryFailureHandler(QueueId queueId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Create Queue Adapter Factory.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="name">The name.</param>
    /// <returns>The queue adapter factory.</returns>
    internal static IQueueAdapterFactory Create(IServiceProvider serviceProvider, string name)
    {
        return new DefaultQueueAdapterFactory(name, null!);
    }

    private void UpdateOptions(object value, string name)
    {
    }
}
