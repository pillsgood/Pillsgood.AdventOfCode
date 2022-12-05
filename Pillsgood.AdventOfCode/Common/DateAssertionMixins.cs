using FluentAssertions.Primitives;

namespace Pillsgood.AdventOfCode.Common;

internal static class DateAssertionMixins
{
    public static void BeAdventDay(this DateOnlyAssertions date)
    {
        date.HaveMonth(12).And.BeAfter(new DateOnly(2015, 11, 30));
    }
}