using System;

namespace Pillsgood.AdventOfCode.Core
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AocYearAttribute : Attribute
    {
        public int Year { get; }

        public AocYearAttribute(int year)
        {
            Year = year;
        }
    }
}