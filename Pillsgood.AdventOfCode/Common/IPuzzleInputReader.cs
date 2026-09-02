using System.Collections;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Pillsgood.AdventOfCode.Common;

public interface IPuzzleInputReader
{
    T Read<T>(TextReader reader, object[]? arguments = null);
}

internal sealed class PuzzleInputReader : IPuzzleInputReader
{
    private readonly IServiceProvider _serviceProvider;

    public PuzzleInputReader(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IEnumerable<string> ReadLines(TextReader reader)
    {
        while (reader.ReadLine() is { } line)
        {
            yield return line.Trim();
        }
    }


    public T Read<T>(TextReader reader, object[]? arguments)
    {
        var expectedType = typeof(T);
        if (expectedType.IsAssignableTo(typeof(IEnumerable)))
        {
            Type? elementType = null;
            elementType ??= expectedType.HasElementType ? expectedType.GetElementType() : null;
            elementType ??= expectedType.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                ?.GetGenericArguments()[0];

            if (elementType == null)
            {
                throw new InvalidOperationException();
            }

            var inputStream = ReadLines(reader);
            var converter = _serviceProvider.GetRequiredService(typeof(ICollectionConverterService<,>).MakeGenericType(typeof(T), elementType));

            return ((ICollectionConverterService<T>)converter).Convert(inputStream, arguments);
        }
        else
        {
            var text = reader.ReadToEnd().Trim();
            var converter = _serviceProvider.GetRequiredService<IConverterService<T>>();
            return converter.Convert(text, arguments);
        }
    }
}