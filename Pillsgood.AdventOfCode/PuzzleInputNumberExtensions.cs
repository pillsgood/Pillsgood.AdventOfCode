using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Common;

namespace Pillsgood.AdventOfCode;

public static class PuzzleInputServiceMixins
{
    extension(IPuzzleInputService inputService)
    {
        public T Get<T>()
        {
            var date = MetadataResolver.ResolveDate(new StackTrace());

            using var stream = inputService.GetInputStreamAsync(date).GetAwaiter().GetResult();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var converter = Locator.Current.GetRequiredService<IPuzzleInputConverter<T>>();
            return converter.Convert(reader);
        }

        public async ValueTask<T> GetAsync<T>()
        {
            var date = MetadataResolver.ResolveDate(new StackTrace());

            await using var stream = await inputService.GetInputStreamAsync(date);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var converter = Locator.Current.GetRequiredService<IPuzzleInputConverter<T>>();
            return converter.Convert(reader);
        }

        public IEnumerable<T> Get<T>(NumberStyles style,
            IFormatProvider? formatProvider = null) where T : INumber<T>
        {
            var date = MetadataResolver.ResolveDate(new StackTrace());

            using var stream = inputService.GetInputStreamAsync(date).GetAwaiter().GetResult();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var converterFactory = Locator.Current.GetRequiredService<NumberConverterFactory<T>>();

            var converter = converterFactory(style, formatProvider);
            return converter.Convert(reader);
        }

        public async ValueTask<IEnumerable<T>> GetAsync<T>(NumberStyles style,
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
}