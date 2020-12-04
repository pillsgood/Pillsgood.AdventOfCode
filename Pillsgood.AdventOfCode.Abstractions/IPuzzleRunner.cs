using System;
using System.Collections.Generic;

namespace Pillsgood.AdventOfCode.Abstractions
{
    public interface IPuzzleRunner
    {
        IEnumerable<KeyValuePair<PuzzleInfo, string[]>> Run(int? year = null, int? day = null);
        IServiceProvider ServiceProvider { get; }
    }
}