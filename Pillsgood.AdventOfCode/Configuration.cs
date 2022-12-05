using System.Reflection;
using Newtonsoft.Json;
using Pillsgood.AdventOfCode.Common;
using Splat;

namespace Pillsgood.AdventOfCode;

public class Configuration
{
    [JsonIgnore] public Assembly EntryAssembly { get; private set; }

    public string? ApplicationName { get; private set; } = Assembly.GetEntryAssembly()?.FullName;

    public Configuration(Assembly entryAssembly)
    {
        EntryAssembly = entryAssembly;
    }

    public Configuration WithApplicationName(string applicationName)
    {
        ApplicationName = applicationName;
        return this;
    }

    public Configuration WithSession(string session)
    {
        var provider = new Lazy<ISessionProvider>(() => new StaticSessionProvider(session));
        Locator.CurrentMutable.Register(() => provider.Value);
        return this;
    }

    public Configuration WithSessionProvider(Func<ISessionProvider> factory)
    {
        Locator.CurrentMutable.RegisterLazySingleton(factory);
        return this;
    }

    public Configuration SetEntryAssembly(Assembly assembly)
    {
        EntryAssembly = assembly;
        return this;
    }
}