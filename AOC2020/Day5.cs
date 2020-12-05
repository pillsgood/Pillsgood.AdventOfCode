using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day5 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => new PuzzleInput<IEnumerable<Seat>>(provider, Process).Value);
        }

        private static IEnumerable<Seat> Process(string value)
        {
            return value.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim())
                .Select(line => new Seat(line.ToArray()));
        }

        private class Seat
        {
            private readonly Position _x = new Position(0, 7);
            private readonly Position _y = new Position(0, 127);
            private readonly Lazy<int> _seatId;

            public Seat(IEnumerable<char> partition)
            {
                foreach (var c in partition)
                {
                    switch (c)
                    {
                        case 'F':
                            _y.LowerHalf();
                            break;
                        case 'B':
                            _y.UpperHalf();
                            break;
                        case 'L':
                            _x.LowerHalf();
                            break;
                        case 'R':
                            _x.UpperHalf();
                            break;
                        default:
                            throw new Exception("Failed to parse partition");
                    }
                }

                _seatId = new Lazy<int>(() => (int) _y * 8 + (int) _x);
            }

            public int SeatId => _seatId.Value;
        }

        private class Position
        {
            private int _lower;
            private int _upper;

            public static explicit operator int(Position position) => position._lower == position._upper
                ? position._lower
                : throw new ArgumentException("Position is not correct");

            public Position(int start, int end)
            {
                _lower = start;
                _upper = end;
            }

            public void UpperHalf()
            {
                _lower += (_upper - _lower + 1) / 2;
            }

            public void LowerHalf()
            {
                _upper -= (_upper - _lower + 1) / 2;
            }
        }

        [Part(1)]
        private string Part1(IEnumerable<Seat> seats)
        {
            var answer = seats.Max(seat => seat.SeatId);
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(IEnumerable<Seat> seats)
        {
            var sorted = seats.Select(seat => seat.SeatId).ToList();
            sorted.Sort();
            var foundSeat = new List<int>();
            for (var i = sorted.First(); i < sorted.Last(); i++)
            {
                if (!sorted.Contains(i) && sorted.Contains(i + 1) && sorted.Contains(i - 1))
                {
                    foundSeat.Add(i);
                }
            }

            if (foundSeat.Count == 1)
            {
                var answer = foundSeat[0];
                return answer.ToString();
            }

            return null;
        }
    }
}