namespace Pillsgood.AdventOfCode.Abstractions
{
    public class PuzzleData
    {
        internal PuzzleData(IPuzzleMetadata metadata)
        {
            Day = metadata.Day;
            Year = metadata.Year;
        }

        public int Day { get; }
        public int Year { get; }
    }
}