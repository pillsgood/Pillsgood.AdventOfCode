namespace Pillsgood.AdventOfCode.Common.InputConverters;

public class ElementConverter<TSource, TDestination> : IInputConverter<IEnumerable<TSource>, IEnumerable<TDestination>>
{
    private readonly IEnumerable<IInputConverter<TSource, TDestination>> _converters;

    public ElementConverter(IEnumerable<IInputConverter<TSource, TDestination>> converters)
    {
        _converters = converters;
    }

    public int GetAffinity(IEnumerable<TSource> input, object[]? arguments) => 1;

    public IEnumerable<TDestination> Convert(IEnumerable<TSource> input, object[]? arguments)
    {
        foreach (var item in input)
        {
            var converter = _converters.MaxBy(c => c.GetAffinity(item, arguments)) ?? throw new InvalidOperationException();
            yield return converter.Convert(item, arguments);
        }
    }
}