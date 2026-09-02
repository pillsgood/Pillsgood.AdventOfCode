using System.Reflection;

namespace Pillsgood.AdventOfCode.Common.InputConverters;

public class ArrayCollectionConverter<TCollection, TElement> : ICollectionConverter<TCollection, TElement> where TCollection : IEnumerable<TElement>
{
    public int GetAffinity(IEnumerable<TElement> input, object[]? arguments)
    {
        return typeof(TCollection) == typeof(TElement[]) ? 10 : 0;
    }

    public TCollection Convert(IEnumerable<TElement> input, object[]? arguments)
    {
        TElement[] array = [.. input];
        return (TCollection)(object)array;
    }
}

public class ReflectionCollectionConverter<TCollection, TElement> : ICollectionConverter<TCollection, TElement>
    where TCollection : IEnumerable<TElement>
{
    private readonly ConstructorInfo? _constructor;

    public ReflectionCollectionConverter()
    {
        var constructors = typeof(TCollection).GetConstructors();
        _constructor = constructors.FirstOrDefault(x => x.GetParameters() is [var pi] && pi.ParameterType == typeof(IEnumerable<TElement>));
    }

    public int GetAffinity(IEnumerable<TElement> input, object[]? arguments)
    {
        return _constructor != null ? 1 : 0;
    }

    public TCollection Convert(IEnumerable<TElement> input, object[]? arguments)
    {
        if (_constructor == null)
        {
            throw new InvalidOperationException();
        }

        return (TCollection)_constructor.Invoke([input]);
    }
}