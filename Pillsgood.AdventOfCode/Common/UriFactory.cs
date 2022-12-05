namespace Pillsgood.AdventOfCode.Common;

internal static class UriFactory
{
    public static Uri GetPuzzle(DateOnly date)
    {
        return new Uri($"https://adventofcode.com/{date.Year}/day/{date.Day}");
    }

    public static Uri GetPuzzleInput(DateOnly date)
    {
        return new Uri($"https://adventofcode.com/{date.Year}/day/{date.Day}/input");
    }

    public static Uri GetPuzzleSubmit(DateOnly date)
    {
        return new Uri($"https://adventofcode.com/{date.Year}/day/{date.Day}/answer");
    }
}