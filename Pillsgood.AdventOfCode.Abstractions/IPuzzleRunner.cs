using System;
using System.Collections.Generic;

namespace Pillsgood.AdventOfCode.Abstractions
{
    public interface IPuzzleRunner
    {
        IEnumerable<PuzzleData> Run(int? year = null, int? day = null);
        IServiceProvider ServiceProvider { get; }
    }
}