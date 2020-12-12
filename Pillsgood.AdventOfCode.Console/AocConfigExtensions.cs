using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.Extensions.Logging;


namespace Pillsgood.AdventOfCode.Console
{
    public static class AocConfigExtensions
    {
        public static AocConfigBuilder AddConsole(this AocConfigBuilder aocConfig,
            Action<AocConsoleConfig> configure = null)
        {
            var config = new AocConsoleConfig();
            configure?.Invoke(config);

            aocConfig.ConfigureServices(collection =>
            {
                collection.AddLogging(builder => builder.AddAnsiConsoleWriter().SetMinimumLevel(config.LoggingLevel));
                collection.Configure<AnsiConsoleLoggerOptions>(options => options.TimeoutDuration = 500);
                collection.TryAddSingleton<IAocConsole, AocConsole>();
                collection.TryAddSingleton(config);
            });

            aocConfig.PostConfigure(collection =>
            {
                if (collection.Any(descriptor => descriptor.ServiceType == typeof(IAocWebSession)))
                {
                    collection.PostConfigure<AnsiConsoleLoggerOptions>(options => options.TimeoutDuration = 1000);
                }
            });

            return aocConfig;
        }
    }
}