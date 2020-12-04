using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode;
using Pillsgood.AdventOfCode.Console;

namespace AOC2020
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var aoc = AdventOfCode.Build(new AocConfig
            {
                ConfigureServices = ConfigureServices,
            }.AddConsole());

            var runner = aoc.Load();
            runner.Run();
            
            // in case program terminates before console can finish writing
            // await Task.Delay(-1);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
        }
    }
}