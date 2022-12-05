namespace Pillsgood.AdventOfCode.Common.InputConverters;

internal class StringInputConverter : IPuzzleInputConverter<string>
{
    public string Convert(TextReader reader)
    {
        return reader.ReadToEnd();
    }

    public async ValueTask<string> ConvertAsync(TextReader reader)
    {
        return await reader.ReadToEndAsync();
    }
}