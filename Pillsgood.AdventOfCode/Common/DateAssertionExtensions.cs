using AwesomeAssertions.Primitives;

namespace Pillsgood.AdventOfCode.Common;

internal static class DateAssertionExtensions
{
    extension(DateOnlyAssertions date)
    {
        public void BeAdventDay()
        {
            date.HaveMonth(12).And.BeAfter(new DateOnly(2015, 11, 30));
        }
    }
}