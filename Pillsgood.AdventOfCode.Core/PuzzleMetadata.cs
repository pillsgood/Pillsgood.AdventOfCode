using System;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    public class PuzzleMetadata : IPuzzleMetadata
    {
        internal Type Type { get; set; }
        internal int? day = null;
        internal int? year = null;

        public int Day
        {
            get => day ?? throw new ArgumentNullException(
                $"Failed to get day of {Type.Name}. use PuzzleAttribute or rename class to Day** for inference");
            set => day = value;
        }

        public int Year
        {
            get => year ?? throw new ArgumentNullException(
                $"Failed to get year of {Type.Name}. define year in AocConfig or using AocYear assembly attribute; year can also be inferred from namespace/assembly name")
            ;
            set => year = value;
        }
    }
}