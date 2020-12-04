using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;


namespace AOC2020
{
    public partial class Day4 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPuzzleInput<IEnumerable<Dictionary<string, string>>>>(provider =>
                new PuzzleInput<IEnumerable<Dictionary<string, string>>>(provider, Process));
        }

        private static IEnumerable<Dictionary<string, string>> Process(string value)
        {
            return value.Split("\n\n").Select(s => s.Trim())
                .Select(passport => passport.Split('\n').SelectMany(s => s.Split(' '))).Select(entries =>
                    entries.Select(s =>
                    {
                        var pairs = s.Split(':');
                        return new KeyValuePair<string, string>(pairs[0], pairs[1]);
                    }).ToDictionary(pair => pair.Key, pair => pair.Value));
        }

        private readonly Dictionary<string, Func<string, bool>> _passportFields =
            new Dictionary<string, Func<string, bool>>
            {
                {
                    "byr", s => ValidateInt(s, 1920, 2002)
                },
                {
                    "iyr", s => ValidateInt(s, 2010, 2020)
                },
                {
                    "eyr", s => ValidateInt(s, 2020, 2030)
                },
                {
                    "hgt", s => s[^2..] switch
                    {
                        "cm" => ValidateInt(s[..^2], 150, 193),
                        "in" => ValidateInt(s[..^2], 59, 76),
                        _ => false
                    }
                },
                {
                    "hcl", s => s[0] == '#'
                                && s[1..].Length == 6
                                && s[1..].All(c => char.IsDigit(c) || c >= 'a' && c <= 'f')
                },
                {
                    "ecl", s => new[] { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" }.Contains(s)
                },
                {
                    "pid", s => s.Length == 9 && s.All(char.IsDigit)
                },
                {
                    "cid", s => true
                },
            };

        private static bool ValidateInt(string s, int start, int end) =>
            int.TryParse(s, out var digit) && digit >= start && digit <= end;


        [Part(1)]
        private string Part1(IPuzzleInput<IEnumerable<Dictionary<string, string>>> input)
        {
            var requiredFields = _passportFields.Keys.Where(s => s != "cid");
            var validCount = input.Value.Count(passport => requiredFields.All(passport.ContainsKey));
            var answer = validCount.ToString();
            return answer;
        }

        [Part(2)]
        private string Part2(IPuzzleInput<IEnumerable<Dictionary<string, string>>> input)
        {
            var requiredFields = _passportFields.Keys.Where(s => s != "cid");
            var validCount = input.Value.Count(passport =>
                requiredFields.All(passport.ContainsKey) &&
                passport.Keys.All(key => _passportFields[key].Invoke(passport[key])));
            var answer = validCount.ToString();
            return answer;
        }
    }
}