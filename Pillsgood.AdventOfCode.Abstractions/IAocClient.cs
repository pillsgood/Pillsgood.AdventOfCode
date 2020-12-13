﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pillsgood.AdventOfCode.Abstractions
{
    internal interface IAocClient
    {
        Task<string> GetPuzzleInput(IPuzzleMetadata metadata);
        string SessionId { get; }
    }
}