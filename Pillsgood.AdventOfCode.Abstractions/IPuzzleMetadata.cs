using System;
using System.Collections.Generic;

namespace Pillsgood.AdventOfCode.Abstractions
{
    internal interface IPuzzleMetadata : IEqualityComparer<IPuzzleMetadata>
    {
        public int Day { get; }
        public int Year { get; }

        bool IEqualityComparer<IPuzzleMetadata>.Equals(IPuzzleMetadata x, IPuzzleMetadata y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Day == y.Day && x.Year == y.Year;
        }

        int IEqualityComparer<IPuzzleMetadata>.GetHashCode(IPuzzleMetadata obj)
        {
            return HashCode.Combine(obj.Day, obj.Year);
        }
    }
}