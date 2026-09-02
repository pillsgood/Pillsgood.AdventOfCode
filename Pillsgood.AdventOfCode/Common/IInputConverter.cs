namespace Pillsgood.AdventOfCode.Common;

public interface IInputConverter<in TSource, out TDestination>
{
    int GetAffinity(TSource input, object[]? arguments);

    TDestination Convert(TSource input, object[]? arguments);
}

public interface ICollectionConverter<out TCollection, in TElement> : IInputConverter<IEnumerable<TElement>, TCollection>
    where TCollection : IEnumerable<TElement>;