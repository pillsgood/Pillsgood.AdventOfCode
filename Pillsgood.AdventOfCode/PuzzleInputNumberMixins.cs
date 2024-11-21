using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Text;
using Pillsgood.AdventOfCode.Common;
using Splat;

namespace Pillsgood.AdventOfCode;

public static class PuzzleInputServiceMixins
{
    public static T Get<T>(this IPuzzleInputService inputService)
    {
        var date = MetadataResolver.ResolveDate(new StackTrace());

        using var stream = inputService.GetInputStreamAsync(date).GetAwaiter().GetResult();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var converter = Locator.Current.GetRequiredService<IPuzzleInputConverter<T>>();
        return converter.Convert(reader);
    }

    public static async ValueTask<T> GetAsync<T>(this IPuzzleInputService inputService)
    {
        var date = MetadataResolver.ResolveDate(new StackTrace());

        await using var stream = await inputService.GetInputStreamAsync(date);
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var converter = Locator.Current.GetRequiredService<IPuzzleInputConverter<T>>();
        return converter.Convert(reader);
    }

    public static IEnumerable<T> Get<T>(
        this IPuzzleInputService inputService,
        NumberStyles style,
        IFormatProvider? formatProvider = null) where T : INumber<T>
    {
        var date = MetadataResolver.ResolveDate(new StackTrace());

        using var stream = inputService.GetInputStreamAsync(date).GetAwaiter().GetResult();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var converterFactory = Locator.Current.GetRequiredService<NumberConverterFactory<T>>();

        var converter = converterFactory(style, formatProvider);
        return converter.Convert(reader);
    }

    public static async ValueTask<IEnumerable<T>> GetAsync<T>(
        this IPuzzleInputService inputService,
        NumberStyles style,
        IFormatProvider? formatProvider = null) where T : INumber<T>
    {
        var date = MetadataResolver.ResolveDate(new StackTrace());

        await using var stream = await inputService.GetInputStreamAsync(date);
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var converterFactory = Locator.Current.GetRequiredService<NumberConverterFactory<T>>();

        var converter = converterFactory(style, formatProvider);
        return converter.Convert(reader);
    }
}