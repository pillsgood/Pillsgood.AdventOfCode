using System.Reflection;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using NeoSmart.Caching.Sqlite;
using Pillsgood.AdventOfCode.Common;
using Pillsgood.AdventOfCode.Common.InputConverters;
using SQLitePCL;

namespace Pillsgood.AdventOfCode;

public static class Aoc
{
    public static IDisposable Start(Action<Configuration> configure)
    {
        var services = new ServiceCollection();

        var config = new Configuration(services)
        {
            EntryAssembly = Assembly.GetCallingAssembly(),
        };

        configure(config);

        var cachePath = Path.GetFullPath(config.CachePath);
        var cacheDir = Path.GetDirectoryName(cachePath) ?? throw new InvalidOperationException("Unable to get cache directory.");

        if (!Directory.Exists(cacheDir)) Directory.CreateDirectory(cacheDir);

        Batteries_V2.Init();
        services.AddSqliteCache(cachePath, null!);
        services.AddHybridCache(opt =>
        {
            opt.DefaultEntryOptions = new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(30) };
        });

        services.AddSingleton(config);

        services.AddSingleton<SessionService>();
        services.AddSingleton<HttpClient>();
        services.AddSingleton<HttpService>();

        services.AddInputServices();
        services.AddAssertionServices();
        services.AddInputConverters();

        return Locator.Setup(services);
    }
}