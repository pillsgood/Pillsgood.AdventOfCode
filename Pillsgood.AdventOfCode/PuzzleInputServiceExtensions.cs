using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Common;

namespace Pillsgood.AdventOfCode;

public static class PuzzleInputServiceExtensions
{
    extension(IPuzzleInputService inputService)
    {
        public T Get<T>()
        {
            var date = MetadataResolver.ResolveDate(new StackTrace());

            using var stream = inputService.GetInputStreamAsync(date).GetAwaiter().GetResult();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var inputReader = Locator.Current.GetRequiredService<IPuzzleInputReader>();
            return inputReader.Read<T>(reader);
        }

        public async ValueTask<T> GetAsync<T>()
        {
            var date = MetadataResolver.ResolveDate(new StackTrace());

            await using var stream = await inputService.GetInputStreamAsync(date);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var inputReader = Locator.Current.GetRequiredService<IPuzzleInputReader>();
            return inputReader.Read<T>(reader);
        }

        public T Get<T>(NumberStyles style)
        {
            var date = MetadataResolver.ResolveDate(new StackTrace());

            using var stream = inputService.GetInputStreamAsync(date).GetAwaiter().GetResult();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var inputReader = Locator.Current.GetRequiredService<IPuzzleInputReader>();
            return inputReader.Read<T>(reader, [style]);
        }
    }
}