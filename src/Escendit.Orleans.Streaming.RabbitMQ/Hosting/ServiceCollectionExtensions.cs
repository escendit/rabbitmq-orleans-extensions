// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Microsoft.Extensions.DependencyInjection;

using System.Diagnostics.CodeAnalysis;
using Escendit.Orleans.Streaming.RabbitMQ.Hosting;
using Escendit.Orleans.Streaming.RabbitMQ.Options;
using Options;
using Orleans.Configuration;
using Orleans.Runtime;
using RabbitMQ.Client;

/// <summary>
/// Service Collection Extensions.
/// </summary>
[DynamicallyAccessedMembers(
    DynamicallyAccessedMemberTypes.All)]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Named Rabbit MQ.
    /// </summary>
    /// <param name="services">The initial service collection.</param>
    /// <param name="name">The name.</param>
    /// <returns>The updated rabbit mq connection factory builder.</returns>
    public static IRabbitConnectionFactoryBuilder AddRabbitMq(this IServiceCollection services, string name)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(name);

        services
            .AddRabbitNamedConnectionFactory(name);

        return new RabbitConnectionFactoryBuilder(services, name);
    }

    /// <summary>
    /// Add Named Rabbit MQ.
    /// </summary>
    /// <param name="services">The initial service collection.</param>
    /// <param name="name">The name.</param>
    /// <param name="options">The options.</param>
    /// <returns>The updated rabbit mq connection factory builder.</returns>
    public static IRabbitConnectionFactoryBuilder AddRabbitMq(
        this IServiceCollection services,
        string name,
        Action<RabbitOptionsBase> options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddRabbitMq(name, configure =>
            {
                configure.Configure(options);
            });

        return new RabbitConnectionFactoryBuilder(services, name);
    }

    /// <summary>
    /// Add Named Rabbit MQ.
    /// </summary>
    /// <param name="services">The initial service collection.</param>
    /// <param name="name">The name.</param>
    /// <param name="configureOptions">The configure options.</param>
    /// <returns>The updated rabbit mq connection factory builder.</returns>
    public static IRabbitConnectionFactoryBuilder AddRabbitMq(
        this IServiceCollection services,
        string name,
        Action<OptionsBuilder<RabbitOptionsBase>> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configureOptions);

        configureOptions
            .Invoke(services.AddOptions<RabbitOptionsBase>(name));

        services
            .ConfigureNamedOptionForLogging<RabbitOptionsBase>(name)
            .AddRabbitNamedConnectionFactory(name);
        return new RabbitConnectionFactoryBuilder(services, name);
    }

    /// <summary>
    /// Use with single connection.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void WithConnection(this IRabbitConnectionFactoryBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .Services
            .AddRabbitNamedConnection(builder.Name);
    }

    /// <summary>
    /// Add Rabbit MQ Named Connection Factory.
    /// </summary>
    /// <param name="services">The initial service collection.</param>
    /// <param name="name">The name.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddRabbitNamedConnectionFactory(this IServiceCollection services, string name)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(name);

        return services
            .AddSingletonNamedService<IConnectionFactory>(
                name,
                (serviceProvider, providerName) =>
                {
                    var options = serviceProvider.GetOptionsByName<RabbitOptionsBase>(providerName);
                    return new ConnectionFactory
                    {
                        Password = options.Password,
                        UserName = options.UserName,
                        VirtualHost = options.VirtualHost,
                        UseBackgroundThreadsForIO = true,
                        DispatchConsumersAsync = true,
                        Ssl = options.SslOptions is null
                            ? null
                            : new SslOption
                            {
                                AcceptablePolicyErrors = options.SslOptions.AcceptablePolicyErrors,
                                Certs = options.SslOptions.Certificates,
                                Enabled = options.SslOptions.Enabled,
                                Version = options.SslOptions.Version,
                                CertPassphrase = options.SslOptions.CertPassphrase,
                                CertPath = options.SslOptions.CertPath,
                                ServerName = options.SslOptions.ServerName,
                                CertificateSelectionCallback = options.SslOptions.CertificateSelectionCallback,
                                CertificateValidationCallback = options.SslOptions.CertificateValidationCallback,
                            },
                    };
                });
    }

    /// <summary>
    /// Add Rabbit MQ Named Connection.
    /// </summary>
    /// <param name="services">The initial service collection.</param>
    /// <param name="name">The name.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddRabbitNamedConnection(this IServiceCollection services, string name)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(name);

        return services
            .AddSingletonNamedService<IConnection>(name, (serviceProvider, providerName) =>
            {
                var options = serviceProvider.GetOptionsByName<RabbitOptionsBase>(providerName);
                var clusterOptions = serviceProvider.GetRequiredService<IOptions<ClusterOptions>>();
                var connectionFactory = serviceProvider.GetRequiredServiceByName<IConnectionFactory>(providerName);
                var clusterId = clusterOptions.Value.ClusterId;
                var serviceId = clusterOptions.Value.ServiceId;

                return Task
                    .Run(() =>
                        connectionFactory
                            .CreateConnection(
                                options
                                    .Endpoints
                                    .Select(endpoint =>
                                        new AmqpTcpEndpoint(
                                            endpoint.HostName,
                                            endpoint.Port ?? 5672))
                                    .ToList(),
                                $"queue-client-{clusterId}-{serviceId}-{name}"))
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            });
    }
}
