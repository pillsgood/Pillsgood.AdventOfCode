using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pillsgood.AdventOfCode.Abstractions
{
    public class AocConfig
    {
        internal AocConfig()
        {
        }

        internal int? Year { get; set; } = null;
        internal Action<IServiceCollection> Services { get; set; } = delegate { };
        internal Assembly[] assemblies = new Assembly[0];
        internal LogLevel LoggingLevel { get; set; } = LogLevel.None;
    }
}