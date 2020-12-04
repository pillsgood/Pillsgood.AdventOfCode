using System;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    public class PuzzleInput<T> : IPuzzleInput<T>
    {
        public delegate T FromRaw(string value);

        private readonly Lazy<T> _value;

        public PuzzleInput(IServiceProvider serviceProvider, FromRaw process)
        {
            RawInput = serviceProvider.GetService<IPuzzleInput>();
            _value = new Lazy<T>(() => process(RawInput.Value));
        }

        public IPuzzleInput RawInput { get; }
        public T Value => _value.Value;
    }
}