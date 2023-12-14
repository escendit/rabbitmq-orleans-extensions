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
using RabbitMQ.Provider;
using ConnectionOptions = Extensions.DependencyInjection.RabbitMQ.Abstractions.ConnectionOptions;

/// <summary>
/// Stream Protocol Adapter Factory.
/// </summary>
public sealed class StreamProtocolAdapterFactory : AdapterFactoryBase
{
    private readonly ILoggerFactory _loggerFactory;
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
    /// <param name="streamQueueMapper">stream queue mapper.</param>
    /// <param name="queueAdapterCache">The queue adapter cache.</param>
    /// <param name="connectionOptions">The connection options.</param>
    /// <param name="streamOptions">The stream options.</param>
    /// <param name="clusterOptions">The cluster options.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public StreamProtocolAdapterFactory(
        string name,
        IStreamQueueMapper streamQueueMapper,
        IQueueAdapterCache queueAdapterCache,
        ConnectionOptions connectionOptions,
        StreamOptions streamOptions,
        ClusterOptions clusterOptions,
        Serializer serializer,
        ILoggerFactory loggerFactory)
        : base(loggerFactory.CreateLogger<StreamProtocolAdapterFactory>())
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(streamQueueMapper);
        ArgumentNullException.ThrowIfNull(queueAdapterCache);
        ArgumentNullException.ThrowIfNull(connectionOptions);
        ArgumentNullException.ThrowIfNull(streamOptions);
        ArgumentNullException.ThrowIfNull(clusterOptions);
        ArgumentNullException.ThrowIfNull(serializer);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        _loggerFactory = loggerFactory;
        _name = name;
        _streamQueueMapper = streamQueueMapper;
        _queueAdapterCache = queueAdapterCache;
        _connectionOptions = connectionOptions;
        _clusterOptions = clusterOptions;
        _serializer = serializer.GetSerializer<RabbitMqBatchContainer>();
        _streamFailureHandlerFactory = streamOptions.StreamFailureHandler;
    }

    /// <inheritdoc />
    public override async Task<IQueueAdapter> CreateAdapter()
    {
        LogCreateAdapter(_name);
        var streamSystem = await CreateStreamSystem(_connectionOptions, _loggerFactory.CreateLogger<StreamSystem>());
        return new StreamProtocolAdapter(_name, _loggerFactory, _clusterOptions, _serializer, _streamQueueMapper, streamSystem);
    }

    /// <inheritdoc />
    public override IQueueAdapterCache GetQueueAdapterCache()
    {
        LogGetQueueAdapterCache(_name);
        return _queueAdapterCache;
    }

    /// <inheritdoc />
    public override IStreamQueueMapper GetStreamQueueMapper()
    {
        LogGetStreamQueueMapper(_name);
        return _streamQueueMapper;
    }

    /// <inheritdoc />
    public override Task<IStreamFailureHandler> GetDeliveryFailureHandler(QueueId queueId)
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
        var streamQueueMapper = serviceProvider.GetRequiredOrleansServiceByName<IStreamQueueMapper>(factoryName);
        var queueAdapterCache = serviceProvider.GetRequiredOrleansServiceByName<IQueueAdapterCache>(factoryName);
        var connectionOptions = serviceProvider.GetOptionsByName<ConnectionOptions>(factoryName);
        var streamOptions = serviceProvider.GetOptionsByName<StreamOptions>(factoryName);
        return ActivatorUtilities.CreateInstance<StreamProtocolAdapterFactory>(
            serviceProvider,
            name,
            streamQueueMapper,
            queueAdapterCache,
            connectionOptions,
            streamOptions,
            clusterOptions.Value);
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
}
