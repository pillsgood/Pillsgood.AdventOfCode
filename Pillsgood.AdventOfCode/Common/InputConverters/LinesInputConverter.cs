namespace Pillsgood.AdventOfCode.Common.InputConverters;

internal class LinesInputConverter : IPuzzleInputConverter<IEnumerable<string>>
{
    public IEnumerable<string> Convert(TextReader reader)
    {
        var list = new List<string>();

        while (reader.ReadLine() is { } line)
        {
            list.Add(line);
        }

        return list;
    }
}