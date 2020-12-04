using System;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal static class PuzzleInfoExtensions
    {
        internal static PuzzleInfo ToPuzzleInfo(this IPuzzleInfo puzzleInfo)
        {
            return new PuzzleInfo
            {
                Year = puzzleInfo.Year ?? throw new NullReferenceException("Failed to get year of puzzle"),
                Day = puzzleInfo.Day ?? throw new NullReferenceException("Failed to get day of puzzle")
            };
        }
    }
}