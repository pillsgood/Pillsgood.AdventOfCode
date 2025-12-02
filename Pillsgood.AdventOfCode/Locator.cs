using Microsoft.Extensions.DependencyInjection;

namespace Pillsgood.AdventOfCode;

public static class Locator
{
    private static ServiceProvider? _serviceProvider;

    internal static IServiceProvider Current => _serviceProvider ?? throw new InvalidOperationException("Locator not initialized.");

    public static T GetRequiredService<T>() where T : notnull => Current.GetRequiredService<T>();

    internal static IDisposable Setup(ServiceCollection services)
    {
        _serviceProvider = services.BuildServiceProvider();
        return _serviceProvider;
    }
}