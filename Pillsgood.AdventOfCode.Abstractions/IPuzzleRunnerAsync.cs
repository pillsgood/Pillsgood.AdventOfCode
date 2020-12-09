using System;
using System.Collections.Generic;

namespace Pillsgood.AdventOfCode.Abstractions
{
    public interface IPuzzleRunnerAsync: IServiceProvider
    {
        IAsyncEnumerable<IPuzzleData> RunAsync(int? year = null, int? day = null);
    }
}