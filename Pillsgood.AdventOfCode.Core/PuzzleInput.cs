using System;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PuzzleInput : IPuzzleInput
    {
        private readonly Lazy<string> _value;

        public PuzzleInput(Func<string> value)
        {
            _value = new Lazy<string>(value);
        }

        public string Value => _value.Value;
    }

    public class PuzzleInput<T> : IPuzzleInput<T>
    {
        public delegate T FromRaw(string value);

        private readonly Lazy<T> _value;

        public PuzzleInput(IServiceProvider serviceProvider, FromRaw process)
        {
            RawInput = serviceProvider.GetRequiredService<IPuzzleInput>();
            _value = new Lazy<T>(() => process(RawInput.Value));
        }

        public IPuzzleInput RawInput { get; }
        public T Value => _value.Value;
    }
}