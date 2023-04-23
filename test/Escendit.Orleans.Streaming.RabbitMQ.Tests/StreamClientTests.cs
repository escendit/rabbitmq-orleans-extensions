// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Tests;

using System.Net;
using global::RabbitMQ.Stream.Client;
using Xunit.Abstractions;
using Xunit.Categories;

/// <summary>
/// Stream Client Tests.
/// </summary>
public class StreamClientTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamClientTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper.</param>
    public StreamClientTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Connect.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ConnectAsync()
    {
        var streamSystem = await StreamSystem.Create(new StreamSystemConfig
        {
            Endpoints = new List<EndPoint>
            {
                new DnsEndPoint("localhost", 5552),
            },
            UserName = "test",
            Password = "test",
            VirtualHost = "testing",
        });

        var confirmationCount = 0;

        await streamSystem.CreateStream(new StreamSpec("stream.test")
        {
            MaxSegmentSizeBytes = 20_000_000,
        }).ConfigureAwait(false);

        var rawProducer = await streamSystem.CreateRawProducer(new RawProducerConfig("stream.test"));
        var lastPublishingId = await rawProducer.GetLastPublishingId();
        await rawProducer.Send(lastPublishingId + 1, new Message("I AM A RAW STREAM!"u8.ToArray()));

        /*var producer = await Producer.Create(new ProducerConfig(streamSystem, "stream.test")
        {
            ConfirmationHandler = async confirmation =>
            {
                switch (confirmation.Status)
                {
                    case ConfirmationStatus.Confirmed:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }, new Logger<Producer>(new NullLoggerFactory())).ConfigureAwait(false);

        await producer.Send(new List<Message>
        {
            new("I AM A STREAM!"u8.ToArray()),
        }).ConfigureAwait(false);
        await producer.Close().ConfigureAwait(false);*/
        await streamSystem.Close().ConfigureAwait(false);
    }
}
