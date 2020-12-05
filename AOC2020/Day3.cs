using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public partial class Day3 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<TreeMap>();
        }

        private class TreeMap
        {
            private readonly List<List<char>> _map;
            private int XMax { get; set; }
            private int YMax { get; set; }

            public TreeMap(IPuzzleInput puzzleInput)
            {
                _map = ReadPuzzleInput(puzzleInput.Value);
            }

            private char GetAtPosition(int x, int y)
            {
                if (x < 0 || x >= XMax)
                    throw new ArgumentException("X Index out of range");

                if (y < 0 || y >= YMax)
                    throw new ArgumentException("Y Index out of range");

                return _map[y][x];
            }

            private List<List<char>> ReadPuzzleInput(string input)
            {
                var map = input.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Select(line => line.ToList())
                    .ToList();

                YMax = map.Count;
                XMax = map[0].Count;

                if (map.Any(row => row.Count != XMax))
                {
                    throw new Exception("NOT ALL ROW HAVE THE SAME SIZE");
                }

                return map;
            }

            public long TraverseMapAndCount(int right, int down)
            {
                var i = right;
                var treeCount = 0;
                for (int j = down; j < YMax; j += down, i = (i + right) % XMax)
                {
                    var value = GetAtPosition(i, j);
                    if (value == '#')
                    {
                        treeCount++;
                    }
                }

                return treeCount;
            }
        }


        [Part(1)]
        private string Part1(TreeMap map)
        {
            var answer = map.TraverseMapAndCount(3, 1).ToString();
            return answer;
        }

        [Part(2)]
        private string Part2(TreeMap map)
        {
            var counts = new[]
            {
                map.TraverseMapAndCount(1, 1),
                map.TraverseMapAndCount(3, 1),
                map.TraverseMapAndCount(5, 1),
                map.TraverseMapAndCount(7, 1),
                map.TraverseMapAndCount(1, 2)
            };

            var answer = counts.Aggregate((i, j) => i * j);
            return answer.ToString();
        }
    }
}