using Microsoft.Extensions.DependencyInjection;

namespace Pillsgood.AdventOfCode.Common;

public static class PuzzleInputService
{
    private static readonly Lazy<IPuzzleInputService> InputService = new(
        static () => Locator.Current.GetRequiredService<IPuzzleInputService>());

    public static Task<Stream> GetInputStream(DateOnly date)
    {
        return InputService.Value.GetInputStreamAsync(date);
    }
}