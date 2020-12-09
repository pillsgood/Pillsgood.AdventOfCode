using System.Collections.Generic;

namespace Pillsgood.AdventOfCode.Abstractions
{
    public interface IPuzzleData
    {
        int Day { get; }
        int Year { get; }
        string Title { get; }
        string Input { get; }
        IEnumerable<IPuzzleResult> Results { get; }
    }
}