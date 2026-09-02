using System.Globalization;
using System.Numerics;

namespace Pillsgood.AdventOfCode.Common.InputConverters;

public class NumberInputConverter<TSource, TDestination> : IInputConverter<TSource, TDestination> where TDestination : INumber<TDestination>
{
    public int GetAffinity(TSource input, object[]? arguments)
    {
        return input is string && arguments is [NumberStyles, ..] ? 10 : 0;
    }

    public TDestination Convert(TSource input, object[]? arguments)
    {
        if (input is not string str) throw new ArgumentException("Input must be a string");
        if (arguments is not [NumberStyles numberStyles, ..]) throw new ArgumentException("Arguments must contain a NumberStyles");

        var formatProvider = arguments is [.., IFormatProvider fp] ? fp : null;
        return TDestination.Parse(str, numberStyles, formatProvider);
    }
}

public class ParsableInputConverter<TSource, TDestination> : IInputConverter<TSource, TDestination>
    where TDestination : IParsable<TDestination>
{
    public int GetAffinity(TSource input, object[]? arguments) => input is string ? 5 : 0;

    public TDestination Convert(TSource input, object[]? arguments)
    {
        if (input is not string str) throw new ArgumentException("Input must be a string");

        var formatProvider = arguments is [IFormatProvider fp] ? fp : null;
        return TDestination.Parse(str, formatProvider);
    }
}