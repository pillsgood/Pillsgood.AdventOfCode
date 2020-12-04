using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.Extensions.Logging;

namespace Pillsgood.AdventOfCode.Console
{
    public static class AocConsoleExtensions
    {
        public static AocConfig AddConsole(this AocConfig aocConfig, Action<AocConsoleConfig> configure = null)
        {
            var config = new AocConsoleConfig();
            configure?.Invoke(config);

            aocConfig.ConfigureServices += collection =>
            {
                collection.AddLogging(builder => builder.AddAnsiConsoleWriter());
                collection.TryAddSingleton<IAocConsole, AocConsole>();
                collection.TryAddSingleton(config);
            };

            return aocConfig;
        }
    }
}