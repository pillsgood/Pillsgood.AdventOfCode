using System.Globalization;
using System.Numerics;
using Splat;

namespace Pillsgood.AdventOfCode.Common.InputConverters;

internal class NumberInputConverter<T> : IPuzzleInputConverter<IEnumerable<T>> where T : INumber<T>
{
    private readonly NumberStyles _style;
    private readonly IFormatProvider? _formatProvider;
    private readonly IPuzzleInputConverter<string[]> _converter;

    public NumberInputConverter(NumberStyles style, IFormatProvider? formatProvider)
    {
        _style = style;
        _formatProvider = formatProvider;
        _converter = Locator.Current.GetRequiredService<IPuzzleInputConverter<string[]>>();
    }

    public IEnumerable<T> Convert(TextReader reader)
    {
        var input = _converter.Convert(reader);
        return input.Select(x => T.Parse(x, _style, _formatProvider));
    }

    public async ValueTask<IEnumerable<T>> ConvertAsync(TextReader reader)
    {
        var input = await _converter.ConvertAsync(reader);
        return input.Select(x => T.Parse(x, _style, _formatProvider));
    }
}