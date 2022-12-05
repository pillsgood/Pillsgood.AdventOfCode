using System.Reflection;
using Akavache;
using FluentAssertions;
using Newtonsoft.Json;
using Pillsgood.AdventOfCode.Common;
using Pillsgood.AdventOfCode.Common.InputConverters;
using Splat;

namespace Pillsgood.AdventOfCode;

public static class Registrations
{
    public static Task<IAsyncDisposable> StartAsync(Action<Configuration> configure)
    {
        var config = new Configuration(Assembly.GetCallingAssembly());
        configure(config);

        ValidateConfig(config);
        Serialize(config);

        Akavache.Registrations.Start(config.ApplicationName!);

        var resolver = Locator.CurrentMutable;

        resolver.RegisterConstant(config);

        resolver.Register(() =>
            Locator.Current.GetRequiredService<IAkavacheHttpClientFactory>().CreateClient(string.Empty));
        resolver.RegisterLazySingleton(() => new HttpService());

        // Input Service
        resolver.RegisterLazySingleton<IPuzzleInputService>(static () => new InputService());

        // Assertion Service
        resolver.RegisterLazySingleton<IAnswerService>(static () => new AnswerService());
        resolver.RegisterLazySingleton<IAnswerAssertion>(static () => new Assertion());

        // Input Converters
        resolver.Register<IPuzzleInputConverter<string>, StringInputConverter>();
        resolver.Register<IPuzzleInputConverter<string[]>, LinesInputConverter>();
        resolver.Register<IPuzzleInputConverter<List<string>>, LinesInputConverter<List<string>>>();

        resolver.Register<IPuzzleInputConverter<IList<string>>>(static () =>
            new AnonymousInputConverter<List<string>, IList<string>>(static x => x));

        resolver.Register<NumberConverterFactory<int>>(static () =>
            (style, provider) => new NumberInputConverter<int>(style, provider));
        resolver.Register<NumberConverterFactory<long>>(static () =>
            (style, provider) => new NumberInputConverter<long>(style, provider));
        resolver.Register<NumberConverterFactory<double>>(static () =>
            (style, provider) => new NumberInputConverter<double>(style, provider));
        resolver.Register<NumberConverterFactory<decimal>>(static () =>
            (style, provider) => new NumberInputConverter<decimal>(style, provider));
        resolver.Register<NumberConverterFactory<float>>(static () =>
            (style, provider) => new NumberInputConverter<float>(style, provider));


        return Task.FromResult((IAsyncDisposable)new Shutdown());
    }

    private static void ValidateConfig(Configuration config)
    {
        config.ApplicationName.Should().NotBeNullOrEmpty("ApplicationName must be set.");
    }

    private static void Serialize(Configuration config)
    {
        var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
        var aocDir = new DirectoryInfo(Path.Combine(assemblyPath, ".aoc"));
        aocDir.Create();

        var json = JsonConvert.SerializeObject(config);
        File.WriteAllText(Path.Combine(aocDir.FullName, "config.json"), json);
    }

    private class Shutdown : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            await BlobCache.Shutdown();
        }
    }
}