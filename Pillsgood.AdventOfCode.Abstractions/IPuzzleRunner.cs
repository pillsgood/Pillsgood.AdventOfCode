using System;
using System.Collections.Generic;

namespace Pillsgood.AdventOfCode.Abstractions
{
    public interface IPuzzleRunner : IServiceProvider
    {
        IEnumerable<IPuzzleData> Run(int? year = null, int? day = null);
    }
}