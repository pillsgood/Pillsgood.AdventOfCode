namespace Pillsgood.AdventOfCode.Abstractions
{
    public struct PuzzleInfo
    {
        public PuzzleInfo(int day, int year)
        {
            Day = day;
            Year = year;
        }

        public int Year { get; internal set; }
        public int Day { get; internal set; }
    }
}