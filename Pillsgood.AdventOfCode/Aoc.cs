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

        Batteries_V2.Init();
        services.AddSqliteCache(config.CachePath, null!);
        services.AddHybridCache(opt =>
        {
            opt.DefaultEntryOptions = new HybridCacheEntryOptions { Expiration = null };
        });

        services.AddSingleton(config);

        services.AddSingleton<HttpClient>();
        services.AddSingleton<HttpService>();

        // Input Service
        services.AddSingleton<IPuzzleInputService, InputService>();

        // Assertion Service
        services.AddSingleton<IAnswerService, AnswerService>();
        services.AddSingleton<IAnswerAssertion, Assertion>();

        // Input Converters
        services.AddTransient<IPuzzleInputConverter<string>>(_ => new StringInputConverter());
        services.AddTransient<IPuzzleInputConverter<IEnumerable<string>>>(_ => new LinesInputConverter());
        services.AddTransient(_ => CollectionConverter<string>.Create(x => x.ToList()));
        services.AddTransient(_ => CollectionConverter<string>.Create(x => x.ToArray()));
        services.AddTransient(_ => CollectionConverter<string>.Create(IReadOnlyList<string> (x) => x.ToList()));
        services.AddTransient(_ => CollectionConverter<string>.Create(IReadOnlyCollection<string> (x) => x.ToList()));


        services.AddTransient<NumberConverterFactory<int>>(static _ => (style, provider) => new NumberInputConverter<int>(style, provider));
        services.AddTransient<NumberConverterFactory<long>>(static _ => (style, provider) => new NumberInputConverter<long>(style, provider));
        services.AddTransient<NumberConverterFactory<double>>(static _ => (style, provider) => new NumberInputConverter<double>(style, provider));
        services.AddTransient<NumberConverterFactory<decimal>>(static _ => (style, provider) => new NumberInputConverter<decimal>(style, provider));
        services.AddTransient<NumberConverterFactory<float>>(static _ => (style, provider) => new NumberInputConverter<float>(style, provider));

        return Locator.Setup(services);
    }
}