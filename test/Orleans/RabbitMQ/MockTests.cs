// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace RabbitMQ.Tests;

using Escendit.Orleans.Streaming.RabbitMQ.Provider;
using Microsoft.Extensions.Logging;
using NSubstitute;

/// <summary>
/// Mock Tests.
/// </summary>
public sealed class MockTests : IDisposable
{
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockTests"/> class.
    /// </summary>
    public MockTests()
    {
        _loggerFactory = GetLoggerFactory();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="MockTests"/> class.
    /// </summary>
    ~MockTests() => Dispose();

    /// <summary>
    /// Initial Adapter Factory Base.
    /// </summary>
    [Fact]
    public void InitiateAdapterFactoryBase()
    {
        var adapterFactory = Substitute.For<AdapterFactoryBase>(GetLogger("test"));
        Assert.NotNull(adapterFactory);
    }

    /// <summary>
    /// Initial Adapter Receiver Base.
    /// </summary>
    [Fact]
    public void InitiateAdapterReceiverBase()
    {
        var adapterReceiverBase = Substitute.For<AdapterReceiverBase>(GetLogger("test"));
        Assert.NotNull(adapterReceiverBase);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _loggerFactory.Dispose();
        GC.SuppressFinalize(this);
    }

    private static ILoggerFactory GetLoggerFactory()
    {
        return LoggerFactory.Create(_ => { });
    }

    private ILogger GetLogger(string loggerName)
    {
        return _loggerFactory.CreateLogger(loggerName);
    }
}
