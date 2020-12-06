using System;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PuzzleAttribute : Attribute, IPuzzleMetadata
    {
        internal readonly PuzzleMetadata metadata = new PuzzleMetadata();

        public PuzzleAttribute(int day)
        {
            Day = day;
        }

        public int Day
        {
            get => metadata.Day;
            set => metadata.Day = value;
        }

        public int Year
        {
            get => metadata.Year;
            set => metadata.Year = value;
        }
    }
}