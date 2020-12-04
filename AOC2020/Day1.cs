using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;


namespace AOC2020
{
    public partial class Day1 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPuzzleInput<IEnumerable<int>>>(provider =>
                new PuzzleInput<IEnumerable<int>>(provider, Process));
        }

        private static IEnumerable<int> Process(string value) => value
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim()).Select(int.Parse);


        [Part(1)]
        private string Part1(IPuzzleInput<IEnumerable<int>> input)
        {
            var entries = input.Value.ToArray();
            for (var i = 0; i < entries.Length; i++)
            for (var j = i + 1; j < entries.Length; j++)
            {
                var sum = entries[i] + entries[j];
                if (sum == 2020)
                {
                    var answer = entries[i] * entries[j];
                    return answer.ToString();
                }
            }

            return null;
        }

        [Part(2)]
        private string Part2(IPuzzleInput<IEnumerable<int>> input)
        {
            var entries = input.Value.ToArray();
            for (var i = 0; i < entries.Length; i++)
            for (var j = i + 1; j < entries.Length; j++)
            for (var k = j + 1; k < entries.Length; k++)
            {
                var sum = entries[i] + entries[j] + entries[k];
                if (sum == 2020)
                {
                    var answer = entries[i] * entries[j] * entries[k];
                    return answer.ToString();
                }
            }

            return null;
        }
    }
}