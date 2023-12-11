// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.StreamProtocol.Provider;

using System.Net;
using Configuration;
using Core;
using global::Orleans.Configuration;
using global::Orleans.Configuration.Overrides;
using global::Orleans.Serialization;
using global::Orleans.Streams;
using global::RabbitMQ.Stream.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ConnectionOptions = Extensions.DependencyInjection.RabbitMQ.Abstractions.ConnectionOptions;

/// <summary>
/// Stream Protocol Adapter Factory.
/// </summary>
public sealed partial class StreamProtocolAdapterFactory : IQueueAdapterFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;
    private readonly string _name;
    private readonly ConnectionOptions _connectionOptions;
    private readonly ClusterOptions _clusterOptions;
    private readonly Serializer<RabbitMqBatchContainer> _serializer;
    private readonly IStreamQueueMapper _streamQueueMapper;
    private readonly IQueueAdapterCache _queueAdapterCache;
    private readonly Func<QueueId, Task<IStreamFailureHandler>> _streamFailureHandlerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProtocolAdapterFactory"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="queueAdapterCache">The queue adapter cache.</param>
    /// <param name="options">The options.</param>
    /// <param name="connectionOptions">The connection options.</param>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="streamQueueMapper">stream queue mapper.</param>
    /// <param name="streamFailureHandler">The stream failure handler.</param>
    public StreamProtocolAdapterFactory(
        string name,
        IStreamQueueMapper streamQueueMapper,
        IQueueAdapterCache queueAdapterCache,
        ConnectionOptions connectionOptions,
        ClusterOptions clusterOptions,
        Serializer serializer,
        ILoggerFactory loggerFactory,
        Func<QueueId, Task<IStreamFailureHandler>> streamFailureHandler)
    {
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<StreamProtocolAdapterFactory>();
        _name = name;
        _streamQueueMapper = streamQueueMapper;
        _queueAdapterCache = queueAdapterCache;
        _connectionOptions = connectionOptions;
        _clusterOptions = clusterOptions;
        _serializer = serializer.GetSerializer<RabbitMqBatchContainer>();
        _streamFailureHandlerFactory = streamFailureHandler;
    }

    /// <inheritdoc />
    public async Task<IQueueAdapter> CreateAdapter()
    {
        LogCreateAdapter(_name);
        var streamSystem = await CreateStreamSystem(_connectionOptions, _loggerFactory.CreateLogger<StreamSystem>())
            .ConfigureAwait(false);
        return new StreamProtocolAdapter(_name, _loggerFactory, _clusterOptions, _serializer, _streamQueueMapper, streamSystem);
    }

    /// <inheritdoc />
    public IQueueAdapterCache GetQueueAdapterCache()
    {
        LogGetQueueAdapterCache(_name);
        return _queueAdapterCache;
    }

    /// <inheritdoc />
    public IStreamQueueMapper GetStreamQueueMapper()
    {
        LogGetStreamQueueMapper(_name);
        return _streamQueueMapper;
    }

    /// <inheritdoc />
    public Task<IStreamFailureHandler> GetDeliveryFailureHandler(QueueId queueId)
    {
        LogGetDeliveryFailureHandler(_name, queueId);
        return _streamFailureHandlerFactory(queueId);
    }

    /// <summary>
    /// Create Queue Adapter Factory.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="name">The name.</param>
    /// <returns>The queue adapter factory.</returns>
    internal static StreamProtocolAdapterFactory Create(IServiceProvider serviceProvider, object? name)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(name);

        if (name is not string factoryName)
        {
            throw new ArgumentException("invalid name");
        }

        var clusterOptions = serviceProvider.GetProviderClusterOptions(factoryName);
        return ActivatorUtilities.CreateInstance<StreamProtocolAdapterFactory>(serviceProvider, name, clusterOptions.Value);
    }

    private static Task<StreamSystem> CreateStreamSystem(ConnectionOptions connectionOptions, ILogger<StreamSystem> logger)
    {
        return StreamSystem.Create(BuildStreamSystemConfig(connectionOptions), logger);
    }

    private static StreamSystemConfig BuildStreamSystemConfig(ConnectionOptions options)
    {
        return new StreamSystemConfig
        {
            Endpoints = options
                .Endpoints
                .Select(s => new DnsEndPoint(s.HostName, s.Port ?? 5552) as EndPoint)
                .ToList(),
            Heartbeat = options.Heartbeat,
            Password = options.Password,
            UserName = options.UserName,
            VirtualHost = options.VirtualHost,
            ClientProvidedName = options.ClientProvidedName,
            Ssl = options.SslOptions is null
                ? null
                : new SslOption
                {
                    AcceptablePolicyErrors = options.SslOptions.AcceptablePolicyErrors,
                    ServerName = options.SslOptions.ServerName,
                    CertificateSelectionCallback = options.SslOptions.CertificateSelectionCallback,
                    CertificateValidationCallback = options.SslOptions.CertificateValidationCallback,
                    CertPassphrase = options.SslOptions.CertPassphrase,
                    CertPath = options.SslOptions.CertPath,
                    Certs = options.SslOptions.Certificates,
                    CheckCertificateRevocation = options.SslOptions.CheckCertificateRevocation,
                    Enabled = options.SslOptions.Enabled,
                    Version = options.SslOptions.Version,
                },
        };
    }

    [LoggerMessage(
        EventId = 100,
        EventName = nameof(CreateAdapter),
        Level = LogLevel.Debug,
        Message = "Creating Queue Adapter for ProviderName: {name}")]
    private partial void LogCreateAdapter(string name);

    [LoggerMessage(
        EventId = 101,
        EventName = nameof(GetQueueAdapterCache),
        Level = LogLevel.Debug,
        Message = "Setting Queue Adapter Cache for ProviderName: {name}")]
    private partial void LogGetQueueAdapterCache(string name);

    [LoggerMessage(
        EventId = 102,
        EventName = nameof(GetStreamQueueMapper),
        Level = LogLevel.Debug,
        Message = "Getting Stream Queue Mapper for ProviderName: {name}")]
    private partial void LogGetStreamQueueMapper(string name);

    [LoggerMessage(
        EventId = 500,
        EventName = nameof(GetDeliveryFailureHandler),
        Level = LogLevel.Debug,
        Message = "Getting Delivery Failure Handler for ProviderName: {name}, QueueId: {queueId}")]
    private partial void LogGetDeliveryFailureHandler(string name, QueueId queueId);
}
