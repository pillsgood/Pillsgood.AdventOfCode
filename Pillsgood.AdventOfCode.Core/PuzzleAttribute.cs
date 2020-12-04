using System;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PuzzleAttribute : Attribute, IPuzzleInfo
    {
        public PuzzleAttribute(int day)
        {
            Day = day;
        }

        internal PuzzleAttribute()
        {
        }

        public int? Year { get; set; }
        public int? Day { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() &&
                   ((IPuzzleInfo) this).Equals((IPuzzleInfo) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Year, Day);
        }

        public static bool operator ==(PuzzleAttribute left, PuzzleAttribute right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PuzzleAttribute left, PuzzleAttribute right)
        {
            return !Equals(left, right);
        }
    }
}