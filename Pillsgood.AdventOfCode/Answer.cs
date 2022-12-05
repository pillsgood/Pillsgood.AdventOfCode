using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using FluentAssertions;
using Pillsgood.AdventOfCode.Common;

namespace Pillsgood.AdventOfCode;

public static class AnswerMixins
{
    public static async ValueTask SubmitAsync(this IAnswerAssertion assertion, string value)
    {
        var (date, part) = MetadataResolver.ResolvePart(new StackTrace());
        await assertion.Assert(date, part, value);
    }

    public static void Submit(this IAnswerAssertion assertion, string value)
    {
        var (date, part) = MetadataResolver.ResolvePart(new StackTrace());
        assertion.Assert(date, part, value).GetAwaiter().GetResult();
    }

    public static async Task SubmitAsync<T>(this IAnswerAssertion assertion, T value, string? format = null)
        where T : INumber<T>
    {
        var (date, part) = MetadataResolver.ResolvePart(new StackTrace());
        var formatted = format is null
            ? value.ToString()
            : value.ToString(format, CultureInfo.InvariantCulture);

        formatted.Should().NotBeNullOrEmpty();

        await assertion.Assert(date, part, formatted!);
    }

    public static void Submit<T>(this IAnswerAssertion assertion, T value, string? format = null)
        where T : INumber<T>
    {
        var (date, part) = MetadataResolver.ResolvePart(new StackTrace());
        var formatted = format is null
            ? value.ToString()
            : value.ToString(format, CultureInfo.InvariantCulture);

        formatted.Should().NotBeNullOrEmpty();

        assertion.Assert(date, part, formatted!).GetAwaiter().GetResult();
    }

    public static void Submit(this IAnswerAssertion assertion, Func<string> solution)
    {
        var answer = solution();
        assertion.Submit(answer);
    }

    public static async ValueTask SubmitAsync(this IAnswerAssertion assertion, Func<ValueTask<string>> solution)
    {
        var answer = await solution();
        await assertion.SubmitAsync(answer);
    }

    public static async ValueTask SubmitAsync(this IAnswerAssertion assertion, Func<string> solution)
    {
        var answer = solution();
        await assertion.SubmitAsync(answer);
    }

    public static void Submit<T>(this IAnswerAssertion assertion, Func<T> solution)
        where T : INumber<T>
    {
        var answer = solution();
        assertion.Submit(answer);
    }

    public static async ValueTask SubmitAsync<T>(this IAnswerAssertion assertion, Func<ValueTask<T>> solution)
        where T : INumber<T>
    {
        var answer = await solution();
        await assertion.SubmitAsync(answer);
    }

    public static async ValueTask SubmitAsync<T>(this IAnswerAssertion assertion, Func<T> solution)
        where T : INumber<T>
    {
        var answer = solution();
        await assertion.SubmitAsync(answer);
    }
}