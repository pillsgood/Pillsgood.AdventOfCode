namespace Pillsgood.AdventOfCode.Common.InputConverters;

internal class LinesInputConverter : IPuzzleInputConverter<string[]>
{
    public string[] Convert(TextReader reader)
    {
        var lines = new List<string>();
        while (reader.ReadLine() is { } line)
        {
            lines.Add(line);
        }

        return lines.ToArray();
    }

    public async ValueTask<string[]> ConvertAsync(TextReader reader)
    {
        var lines = new List<string>();
        while (await reader.ReadLineAsync() is { } line)
        {
            lines.Add(line);
        }

        return lines.ToArray();
    }
}

internal class LinesInputConverter<T> : IPuzzleInputConverter<T> where T : IList<string>, new()
{
    public T Convert(TextReader reader)
    {
        var lines = new T();
        while (reader.ReadLine() is { } line)
        {
            lines.Add(line);
        }

        return lines;
    }

    public async ValueTask<T> ConvertAsync(TextReader reader)
    {
        var lines = new T();
        while (await reader.ReadLineAsync() is { } line)
        {
            lines.Add(line);
        }

        return lines;
    }
}