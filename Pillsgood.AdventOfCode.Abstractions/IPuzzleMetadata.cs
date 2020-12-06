using System;

namespace Pillsgood.AdventOfCode.Abstractions
{
    internal interface IPuzzleMetadata : IEquatable<IPuzzleMetadata>
    {
        public int Day { get; }
        public int Year { get; }

        bool IEquatable<IPuzzleMetadata>.Equals(IPuzzleMetadata other)
        {
            return Year == other?.Year && Day == other?.Day;
        }
    }
}