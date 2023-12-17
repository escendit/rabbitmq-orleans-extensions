// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Streaming.RabbitMQ.Hosting;

using Microsoft.Extensions.DependencyInjection;

#if !NET8_0_OR_GREATER
using global::Orleans.Runtime;
#endif

/// <summary>
/// Orleans Extensions.
/// </summary>
internal static class OrleansExtensions
{
    /// <summary>
    /// Register Keyed Singleton Factory.
    /// </summary>
    /// <param name="services">The initial service collection.</param>
    /// <param name="name">The name.</param>
    /// <param name="implementationFactory">The implementation factory.</param>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddOrleansNamedSingletonFactory<TService>(
        this IServiceCollection services,
        string name,
        Func<IServiceProvider, object?, TService> implementationFactory)
        where TService : class
    {
#if NET8_0_OR_GREATER
        return services.AddKeyedSingleton(name, implementationFactory);
#else
        return services.AddSingletonKeyedService<object?, TService>(name, implementationFactory);
#endif
    }

    /// <summary>
    /// Register Keyed Singleton Implementation.
    /// </summary>
    /// <param name="services">The initial service collection.</param>
    /// <param name="name">The name.</param>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TInstance">The instance type.</typeparam>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddOrleansNamedSingleton<TService, TInstance>(
        this IServiceCollection services,
        string name)
        where TInstance : class, TService
        where TService : class
    {
#if NET8_0_OR_GREATER
        return services.AddKeyedSingleton<TService, TInstance>(name);
#else
        return services.AddSingletonKeyedService<object?, TService, TInstance>(name);
#endif
    }
}
