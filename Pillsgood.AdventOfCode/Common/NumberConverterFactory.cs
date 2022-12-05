using System.Globalization;
using System.Numerics;

namespace Pillsgood.AdventOfCode.Common;

internal delegate IPuzzleInputConverter<IEnumerable<T>> NumberConverterFactory<T>(NumberStyles style,
    IFormatProvider? formatProvider) where T : INumber<T>;