using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode;
using Pillsgood.AdventOfCode.Console;

namespace AOC2020
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var aoc = AdventOfCode.Build(config => config
                .ConfigureServices(ConfigureServices)
                .LoadCallingAssembly()
                .AddConsole());

            // adding console with force aggregate results of answers, otherwise iterate through the return value of runner.Run()
            aoc.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
        }
    }
}