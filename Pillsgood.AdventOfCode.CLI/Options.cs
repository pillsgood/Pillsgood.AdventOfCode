﻿using System;
using CommandLine;

namespace Pillsgood.AdventOfCode.CLI
{
    public class Options
    {
        public Options(int? year, int? day, int? part, string aocSessionId)
        {
            Year = year;
            Day = day;
            SessionID = aocSessionId ?? Environment.GetEnvironmentVariable("AOC_SESSION");
        }

        [Option] public int? Year { get; }
        [Option] public int? Day { get; }
        [Option] public string SessionID { get; }
    }
}