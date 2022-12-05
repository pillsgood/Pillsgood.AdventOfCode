namespace Pillsgood.AdventOfCode.Common;

public interface IPuzzleInputService
{
    Task<Stream> GetInputStreamAsync(DateOnly date);
}