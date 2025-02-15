// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Configuration;
using Aksio.Cratis.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up configuration for host.
/// </summary>
public static class ConfigurationHostBuilderExtensions
{
    /// <summary>
    /// Gets the <see cref="IConfiguration"/> object configured using the "<see cref="UseDefaultConfiguration"/>.
    /// </summary>
    public static IConfiguration Configuration { get; }

    static ConfigurationHostBuilderExtensions()
    {
        Configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
              .AddJsonFile("config/appsettings.json", optional: true, reloadOnChange: true)
              .Build();
    }

    /// <summary>
    /// Use default configuration.
    /// </summary>
    /// <param name="builder"><see cref="IHostBuilder"/> to use with.</param>
    /// <returns><see cref="IHostBuilder"/> for continuation.</returns>
    public static IHostBuilder UseDefaultConfiguration(this IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(_ =>
        {
            _.Sources.Clear();
            _.AddConfiguration(Configuration);
        });

        return builder;
    }

    /// <summary>
    /// Use configuration objects through discovery based on objects adorned with <see cref="ConfigurationAttribute"/>.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> to use with.</param>
    /// <param name="types"><see cref="ITypes"/> for type discovery.</param>
    /// <param name="baseRelativePath">Optional base relative path, relative to the current running directory.</param>
    /// <param name="searchSubPaths">Optional search sub paths, relative to the current running directory and the optional baseRelativePath.</param>
    /// <param name="logger">Logger for logging.</param>
    /// <returns><see cref="IServiceCollection"/> for continuation.</returns>
    /// <remarks>
    /// The order of precedence for configuration files is as expected from .NET Configuration.
    /// See more here: https://devblogs.microsoft.com/premier-developer/order-of-precedence-when-configuring-asp-net-core/.
    /// </remarks>
    public static IServiceCollection AddConfigurationObjects(
        this IServiceCollection services,
        ITypes types,
        string baseRelativePath = "",
        IEnumerable<string>? searchSubPaths = default,
        ILogger? logger = default)
    {
        var allSearchSubPaths = new List<string>
            {
                "./"
            };
        if (searchSubPaths is not null)
        {
            allSearchSubPaths.AddRange(searchSubPaths);
        }
        var allSearchPaths = allSearchSubPaths.Select(_ => Path.Combine(Directory.GetCurrentDirectory(), baseRelativePath, _)).Distinct().ToArray();

        ConfigurationObjects.DiscoverAndAddAllConfigurationObjects(services, types, allSearchPaths, logger);

        return services;
    }
}
