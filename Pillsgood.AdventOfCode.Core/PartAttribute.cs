using System;

namespace Pillsgood.AdventOfCode.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PartAttribute : Attribute
    {
        public PartAttribute(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public override string ToString() => Value.ToString();
    }
}