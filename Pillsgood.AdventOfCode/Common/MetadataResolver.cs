using System.Diagnostics;
using System.Text.RegularExpressions;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Pillsgood.AdventOfCode.Common;

internal static partial class MetadataResolver
{
    private static readonly Lazy<Configuration> _config = new(() => Locator.Current.GetRequiredService<Configuration>());

    private static bool TryResolveDate(StackFrame frame, out DateOnly date)
    {
        var type = frame.GetMethod()!.DeclaringType;

        type.Should().NotBeNull();

        var fullname = type.FullName!;
        fullname.Should().NotBeNullOrEmpty("Type name cannot be null or empty.");

        if (MatchYear(fullname) is not { } year)
        {
            date = default;
            return false;
        }

        if (MatchDay(fullname) is not { } day)
        {
            date = default;
            return false;
        }

        day.Should().BeInRange(1, 31, "day should be between 1 and 31");
        date = new DateOnly(year, 12, day);
        return true;
    }

    private static bool TryResolvePart(StackFrame frame, out int part)
    {
        var method = frame.GetMethod()!;

        if (MatchPart(method.Name) is not { } value)
        {
            part = default;
            return false;
        }

        value.Should().BeOneOf([1, 2], "part should be 0 or 1");
        part = value;
        return true;
    }

    public static DateOnly ResolveDate(StackTrace stackTrace)
    {
        foreach (var frame in FilterFrames(stackTrace))
        {
            if (TryResolveDate(frame, out var date))
            {
                return date;
            }
        }

        throw new InvalidOperationException($"Unable to resolve date.");
    }

    public static (DateOnly date, int part) ResolvePart(StackTrace stackTrace)
    {
        foreach (var frame in FilterFrames(stackTrace))
        {
            if (TryResolveDate(frame, out var date) && TryResolvePart(frame, out var part))
            {
                return (date, part);
            }
        }

        throw new InvalidOperationException($"Unable to resolve part.");
    }

    private static IEnumerable<StackFrame> FilterFrames(StackTrace stackTrace)
    {
        var frames = stackTrace.GetFrames();
        frames.Should().NotBeNullOrEmpty();

        var entry = _config.Value.EntryAssembly;
        foreach (var frame in frames)
        {
            var method = frame.GetMethod();
            if (method is null || method.Module.Assembly != entry)
            {
                continue;
            }

            if (HasDefinedAttribute(method.DeclaringType))
            {
                continue;
            }

            yield return frame;
        }
    }

    private static bool HasDefinedAttribute(Type? type) =>
        type != null && (type.IsDefined(typeof(AocFixtureAttribute), true) || HasDefinedAttribute(type.DeclaringType));

    [GeneratedRegex("20(1[6-9]|[2-9][0-9])", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex YearPattern { get; }

    private static int? MatchYear(string value)
    {
        var match = YearPattern.Match(value);
        if (match.Success)
        {
            return int.Parse(match.Value);
        }

        return null;
    }

    [GeneratedRegex(@"Day(\d+)", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex DayPattern { get; }

    private static int? MatchDay(string value)
    {
        var match = DayPattern.Match(value);
        if (match.Success)
        {
            return int.Parse(match.Groups[1].Value);
        }

        return null;
    }

    [GeneratedRegex(@"Part(\d+)", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex PartPattern { get; }

    private static int? MatchPart(string value)
    {
        var match = PartPattern.Match(value);
        if (match.Success)
        {
            return int.Parse(match.Groups[1].Value);
        }

        return null;
    }
}