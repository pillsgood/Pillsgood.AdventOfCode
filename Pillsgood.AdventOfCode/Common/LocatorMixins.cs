using Splat;

namespace Pillsgood.AdventOfCode.Common;

public static class LocatorMixins
{
    public static T GetRequiredService<T>(this IReadonlyDependencyResolver resolver)
    {
        return resolver.GetService<T>() ?? throw new InvalidOperationException($"Could not resolve {typeof(T).Name}.");
    }
}