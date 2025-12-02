using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Common;

namespace Pillsgood.AdventOfCode;

public class Configuration
{
    private readonly ServiceCollection _serviceCollection;

    public Configuration(
        ServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public required Assembly EntryAssembly { get; set; }

    public Configuration WithSession(string session)
    {
        _serviceCollection.AddSingleton(_ => new StaticSessionProvider(session));
        return this;
    }

    public Configuration WithSessionProvider(Func<ISessionProvider> factory)
    {
        _serviceCollection.AddSingleton(factory);
        return this;
    }

    public Configuration SetEntryAssembly(Assembly assembly)
    {
        EntryAssembly = assembly;
        return this;
    }
}