using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public partial class Day2 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(provider =>
                new PuzzleInput<IEnumerable<Password>>(provider, Process).Value);
        }

        private static IEnumerable<Password> Process(string value)
        {
            foreach (var (rangeStart, rangeEnd, password, character) in value
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s =>
                {
                    string[] arg0;
                    string[] arg1;
                    var password = (arg0 = s.Split(':'))[1].Trim();
                    var range = (arg1 = arg0[0].Split(' '))[0].Trim().Split('-');
                    var character = arg1[1].Trim();
                    return (range[0].Trim(), range[1].Trim(), password, character);
                }))
            {
                var policy = new Policy(int.Parse(rangeStart), int.Parse(rangeEnd), character[0]);
                yield return new Password(policy, password);
            }
        }

        private class Password
        {
            public Policy Policy { get; }
            public string Value { get; }

            public Password(Policy policy, string value)
            {
                Policy = policy;
                Value = value;
            }

            public override string ToString() => $"{Policy.RangeStart}-{Policy.RangeEnd} {Policy.Character}: {Value}";
        }

        private class Policy
        {
            public int RangeStart { get; }
            public int RangeEnd { get; }
            public char Character { get; }

            public Policy(int rangeStart, int rangeEnd, char character)
            {
                RangeStart = rangeStart;
                RangeEnd = rangeEnd;
                Character = character;
            }
        }

        [Part(1)]
        private string Part1(IEnumerable<Password> passwords)
        {
            var validCounts = passwords.Count(password =>
            {
                var charCount = password.Value.Count(c => c.Equals(password.Policy.Character));
                return charCount >= password.Policy.RangeStart && charCount <= password.Policy.RangeEnd;
            });

            return validCounts.ToString();
        }

        [Part(2)]
        private string Part2(IEnumerable<Password> passwords)
        {
            var validCounts = passwords.Count(password =>
            {
                var characters = new[]
                    { password.Value[password.Policy.RangeStart - 1], password.Value[password.Policy.RangeEnd - 1] };
                return characters.Count(c => c.Equals(password.Policy.Character)) == 1;
            });

            return validCounts.ToString();
        }
    }
}