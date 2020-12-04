using System;

namespace Pillsgood.AdventOfCode.Abstractions
{
    internal interface IPuzzleInfo : IEquatable<IPuzzleInfo>
    {
        public int? Year { get; }
        public int? Day { get; }

        bool IEquatable<IPuzzleInfo>.Equals(IPuzzleInfo other)
        {
            return Year == other?.Year && Day == other?.Day;
        }
    }

    internal interface IPuzzlePartInfo : IPuzzleInfo, IEquatable<IPuzzlePartInfo>
    {
        public int? Part { get; }
        
        bool IEquatable<IPuzzlePartInfo>.Equals(IPuzzlePartInfo other)
        {
            return Year == other?.Year && Day == other?.Day && Part == other?.Part;
        }
    }
}