using System;
using Autofac.Features.Indexed;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PuzzleInput : IPuzzleInput
    {
        private readonly Lazy<string> _value;

        public PuzzleInput(PuzzleMetadata puzzleMetadata, IIndex<PuzzleMetadata, Lazy<PuzzleData>> puzzleDataIndex)
        {
            _value = new Lazy<string>(() => puzzleDataIndex[puzzleMetadata].Value.Input);
        }

        public string Value => _value.Value;
    }

    public class PuzzleInput<T> : IPuzzleInput<T>
    {
        public delegate T ParseInput(string input);

        private readonly Lazy<T> _value;

        public PuzzleInput(IServiceProvider serviceProvider, ParseInput parse)
        {
            RawInput = serviceProvider.GetRequiredService<IPuzzleInput>();
            _value = new Lazy<T>(() => parse(RawInput.Value));
        }

        public IPuzzleInput RawInput { get; }
        public T Value => _value.Value;
    }
}