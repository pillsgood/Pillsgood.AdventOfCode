namespace Pillsgood.AdventOfCode.Common;

public interface IConverterService<out TOutput>
{
    TOutput Convert(string input, object[]? arguments);
}

internal sealed class ConverterService<T> : IConverterService<T>
{
    private readonly IEnumerable<IInputConverter<string, T>> _converters;

    public ConverterService(IEnumerable<IInputConverter<string, T>> converters)
    {
        _converters = converters;
    }

    public T Convert(string input, object[]? arguments)
    {
        var converter = _converters.MaxBy(x => x.GetAffinity(input, arguments)) ?? throw new InvalidOperationException("No converter found");
        return converter.Convert(input, arguments);
    }
}