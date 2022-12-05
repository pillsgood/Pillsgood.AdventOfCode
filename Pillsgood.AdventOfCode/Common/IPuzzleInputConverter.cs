namespace Pillsgood.AdventOfCode.Common;

internal interface IPuzzleInputConverter<TOutput>
{
    TOutput Convert(TextReader reader);
    ValueTask<TOutput> ConvertAsync(TextReader reader);
}