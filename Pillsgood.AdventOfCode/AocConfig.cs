using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pillsgood.AdventOfCode
{
    public class AocConfig
    {
        public int? Year { get; set; }
        public Action<IServiceCollection> ConfigureServices { get; set; }
        public LogLevel LoggingLevel { get; set; } = LogLevel.None;
    }
}