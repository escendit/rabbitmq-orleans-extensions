// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Providers.Streams.Common;
using Orleans.Streams;
#if !NET8_0_OR_GREATER
using Orleans.Runtime;
#endif

/// <summary>
/// Service Provider Extensions.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Get Required Orleans Service By Name.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="name">The name.</param>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <returns>The service.</returns>
    public static TService GetRequiredOrleansServiceByName<TService>(this IServiceProvider serviceProvider, string name)
#if NET8_0_OR_GREATER
        where TService : notnull
#else
        where TService : class
#endif
    {
        ArgumentNullException.ThrowIfNull(name);
#if NET8_0_OR_GREATER
        return serviceProvider.GetRequiredKeyedService<TService>(name);
#else
        return serviceProvider.GetRequiredServiceByKey<object?, TService>(name);
#endif
    }

    /// <summary>
    /// Get Optional Orleans Service By Name.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="name">The name.</param>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <returns>The service.</returns>
    public static TService? GetOptionalOrleansServiceByName<TService>(this IServiceProvider serviceProvider, string name)
#if !NET8_0_OR_GREATER
        where TService : class
#endif
    {
        ArgumentNullException.ThrowIfNull(name);
#if NET8_0_OR_GREATER
        return serviceProvider.GetKeyedService<TService>(name);
#else
        return serviceProvider.GetServiceByKey<object?, TService>(name);
#endif
    }

    /// <summary>
    /// Create Stream Queue Mapper.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="name">The name.</param>
    /// <returns>The stream queue mapper.</returns>
    /// <exception cref="ArgumentException">The argument exception.</exception>
    internal static IStreamQueueMapper CreateStreamQueueMapper(
        this IServiceProvider serviceProvider,
        object? name)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(name);
        if (name is not string factoryName)
        {
            throw new ArgumentException("Invalid name");
        }

        var streamQueueMapper = serviceProvider
            .GetOptionalOrleansServiceByName<IStreamQueueMapper>(factoryName) ?? new HashRingBasedStreamQueueMapper(
            new HashRingStreamQueueMapperOptions(),
            factoryName);

        return streamQueueMapper;
    }

    /// <summary>
    /// Create Queue Adapter Cache.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="name">The name.</param>
    /// <returns>The queue adapter cache.</returns>
    /// <exception cref="ArgumentException">The argument exception when name is not string.</exception>
    internal static IQueueAdapterCache CreateQueueAdapterCache(
        IServiceProvider serviceProvider,
        object? name)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(name);
        if (name is not string factoryName)
        {
            throw new ArgumentException("Invalid name");
        }

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var adapterCache = serviceProvider
            .GetOptionalOrleansServiceByName<IQueueAdapterCache>(factoryName) ??
                           new SimpleQueueAdapterCache(new SimpleQueueCacheOptions(), factoryName, loggerFactory);

        return adapterCache;
    }

    /// <summary>
    /// Handle Failure.
    /// </summary>
    /// <param name="queueId">The queue.</param>
    /// <returns>The stream failure handler.</returns>
    internal static Task<IStreamFailureHandler> HandleFailure(QueueId queueId)
    {
        return Task.FromResult<IStreamFailureHandler>(new NoOpStreamDeliveryFailureHandler());
    }
}
