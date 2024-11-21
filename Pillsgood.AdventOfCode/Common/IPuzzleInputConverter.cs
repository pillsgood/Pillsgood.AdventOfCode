namespace Pillsgood.AdventOfCode.Common;

public interface IPuzzleInputConverter<out TOutput>
{
    TOutput Convert(TextReader reader);
}