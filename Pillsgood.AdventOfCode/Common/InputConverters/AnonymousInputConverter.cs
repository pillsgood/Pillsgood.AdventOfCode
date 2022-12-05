using Splat;

namespace Pillsgood.AdventOfCode.Common.InputConverters;

internal class AnonymousInputConverter<TInput, TOutput> : IPuzzleInputConverter<TOutput>
{
    private readonly Func<TInput, TOutput> _convert;
    private readonly IPuzzleInputConverter<TInput> _converter;

    public AnonymousInputConverter(Func<TInput, TOutput> convert)
    {
        _convert = convert;
        _converter = Locator.Current.GetRequiredService<IPuzzleInputConverter<TInput>>();
    }

    public TOutput Convert(TextReader reader)
    {
        var input = _converter.Convert(reader);
        return _convert(input);
    }

    public async ValueTask<TOutput> ConvertAsync(TextReader reader)
    {
        var input = await _converter.ConvertAsync(reader);
        return _convert(input);
    }
}