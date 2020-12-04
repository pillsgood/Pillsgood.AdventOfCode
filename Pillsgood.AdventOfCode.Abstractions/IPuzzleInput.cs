namespace Pillsgood.AdventOfCode.Abstractions
{
    public interface IPuzzleInput
    {
        string Value { get; }
    }

    public interface IPuzzleInput<out T>
    {
        IPuzzleInput RawInput { get; }
        T Value { get; }
    }
}