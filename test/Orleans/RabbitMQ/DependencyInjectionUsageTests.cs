// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace RabbitMQ.Tests;

using Escendit.Orleans.Streaming.RabbitMQ.Hosting;
using Microsoft.Extensions.Hosting;
using Xunit;
using Xunit.Categories;

/// <summary>
/// Dependency Injection Usage Tests.
/// </summary>
public class DependencyInjectionUsageTests
{
    /// <summary>
    /// Start.
    /// </summary>
    [Fact]
    [UnitTest]
    public void Start()
    {
        var host = Host.CreateDefaultBuilder()
            .UseOrleans(siloBuilder => siloBuilder.UseLocalhostClustering())
            .UseOrleans(siloBuilder => siloBuilder
                .AddRabbitMq("Name")
                .Build())
            .Build();
        Assert.NotNull(host);
    }
}
