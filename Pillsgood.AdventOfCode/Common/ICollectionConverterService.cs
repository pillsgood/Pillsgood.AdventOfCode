using Pillsgood.AdventOfCode.Common.InputConverters;

namespace Pillsgood.AdventOfCode.Common;

public interface ICollectionConverterService<out TOutput>
{
    TOutput Convert(IEnumerable<string> input, object[]? arguments = null);
}

public interface ICollectionConverterService<out TOutput, TElement> : ICollectionConverterService<TOutput>
    where TOutput : IEnumerable<TElement>;

internal sealed class CollectionConverterService<TOutput, TElement> : ICollectionConverterService<TOutput, TElement> where TOutput : IEnumerable<TElement>
{
    private readonly ElementConverter<string, TElement> _elementConverter;
    private readonly IEnumerable<IInputConverter<IEnumerable<TElement>, TOutput>> _collectionConverters;

    public CollectionConverterService(
        ElementConverter<string, TElement> elementConverter,
        IEnumerable<ICollectionConverter<TOutput, TElement>> collectionConverters)
    {
        _elementConverter = elementConverter;
        _collectionConverters = collectionConverters;
    }

    public TOutput Convert(IEnumerable<string> input, object[]? arguments)
    {
        var enumerable = _elementConverter.Convert(input, arguments);
        var converter = _collectionConverters.MaxBy(x => x.GetAffinity(enumerable, arguments)) ?? throw new InvalidOperationException();
        return converter.Convert(enumerable, arguments);
    }
}