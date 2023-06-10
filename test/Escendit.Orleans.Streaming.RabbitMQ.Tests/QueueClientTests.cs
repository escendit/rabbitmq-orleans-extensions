// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Tests;

using System.Text;
using global::RabbitMQ.Client;
using Xunit.Categories;

/// <summary>
/// Queue Client Tests.
/// </summary>
public class QueueClientTests
{
    /// <summary>
    /// Connect.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Fact]
    [IntegrationTest]
    public Task ConnectAsync()
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            VirtualHost = "testing",
            UserName = "test",
            Password = "test",
            ClientProvidedName = "queue-client-test",
        };

        var connection = connectionFactory.CreateConnection();
        var model = connection.CreateModel();

        model.ExchangeDeclare("exchange.test", ExchangeType.Fanout, true, false, new Dictionary<string, object>());
        model.QueueDeclare("queue.test", true, false, false, null);
        model.QueueBind("queue.test", "exchange.test", "RK", null);
        model.BasicPublish("exchange.test", "RK", false, null, Encoding.UTF8.GetBytes("THIS I A STRING"));
        Assert.NotNull(connectionFactory);

        return Task.CompletedTask;
    }
}
