﻿// Copyright (c) Escendit Ltd. All Rights Reserved.
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
    /// Create Stream Queue Mapper.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="name">The name.</param>
    /// <returns>The stream queue mapper.</returns>
    /// <exception cref="ArgumentException">The argument exception.</exception>
    internal static IStreamQueueMapper CreateDefaultStreamQueueMapper(
        this IServiceProvider serviceProvider,
        object? name)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(name);

        if (name is not string factoryName)
        {
            throw new ArgumentException("Invalid name");
        }

        var options = serviceProvider.GetOptionsByName<HashRingStreamQueueMapperOptions>(factoryName);
        return new HashRingBasedStreamQueueMapper(options, factoryName);
    }

    /// <summary>
    /// Create Queue Adapter Cache.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="name">The name.</param>
    /// <returns>The queue adapter cache.</returns>
    /// <exception cref="ArgumentException">The argument exception when name is not string.</exception>
    internal static IQueueAdapterCache CreateDefaultQueueAdapterCache(
        this IServiceProvider serviceProvider,
        object? name)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(name);
        if (name is not string factoryName)
        {
            throw new ArgumentException("Invalid name");
        }

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var options = serviceProvider.GetOptionsByName<SimpleQueueCacheOptions>(factoryName);
        return new SimpleQueueAdapterCache(options, factoryName, loggerFactory);
    }
}
