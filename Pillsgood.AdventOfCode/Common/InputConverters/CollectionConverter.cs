using Splat;

namespace Pillsgood.AdventOfCode.Common.InputConverters;

public static class CollectionConverter<TElement>
{
    public static IPuzzleInputConverter<TList> Create<TList>(
        Func<IEnumerable<TElement>, TList> factory) where TList : IEnumerable<TElement>
    {
        return new CollectionConverter<TList, TElement>(factory);
    }
}

internal class CollectionConverter<TList, TElement>
    : IPuzzleInputConverter<TList>
    where TList : IEnumerable<TElement>
{
    private readonly Func<IEnumerable<TElement>, TList> _factory;

    public CollectionConverter(Func<IEnumerable<TElement>, TList> factory)
    {
        _factory = factory;
    }

    private readonly IPuzzleInputConverter<IEnumerable<TElement>> _converter =
        Locator.Current.GetRequiredService<IPuzzleInputConverter<IEnumerable<TElement>>>();

    public TList Convert(TextReader reader)
    {
        return _factory(_converter.Convert(reader));
    }
}