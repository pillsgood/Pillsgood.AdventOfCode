using AwesomeAssertions;
using Microsoft.Extensions.Caching.Hybrid;
using SoftCircuits.HtmlMonkey;

namespace Pillsgood.AdventOfCode.Common;

internal class AnswerService : IAnswerService
{
    private readonly HttpService _httpService;
    private readonly HybridCache _cache;

    public AnswerService(HttpService httpService, HybridCache cache)
    {
        _httpService = httpService;
        _cache = cache;
    }

    public async Task<string?> GetKnownAnswer(DateOnly date, int part)
    {
        try
        {
            var key = GetKey(date, part);
            return await _cache.GetOrCreateAsync(key, FetchFunc);
        }
        catch (KeyNotFoundException)
        {
            return string.Empty;
        }

        async ValueTask<string> FetchFunc(CancellationToken cancellationToken)
        {
            var uri = UriFactory.GetPuzzle(date);
            var html = await _httpService.GetStringAsync(uri);
            var document = await HtmlDocument.FromHtmlAsync(html);
            var answers = HtmlParserService.FindAnswers(document);

            var answer = string.Empty;
            if (part <= answers.Length)
            {
                answer = answers[part - 1];
            }

            if (string.IsNullOrWhiteSpace(answer))
            {
                throw new KeyNotFoundException($"No answer found for {date} part {part}");
            }

            return answer;
        }
    }

    public async Task SubmitAnswer(DateOnly date, int part, string answer)
    {
        var key = GetKey(date, part);
        var previous = await _cache.GetAsync<string>(key);

        if (!string.IsNullOrEmpty(previous))
        {
            answer.Should().BeEquivalentTo(previous);
            return;
        }

        var submitUri = UriFactory.GetPuzzleSubmit(date);
        var html = await _httpService.PostAsync(submitUri, new Dictionary<string, string>()
        {
            ["level"] = part.ToString("0"),
            ["answer"] = answer
        });

        var document = await HtmlDocument.FromHtmlAsync(html);
        if (HtmlParserService.IsAnswerCorrect(document))
        {
            await _cache.SetAsync(key, answer);
            return;
        }

        var reason = HtmlParserService.GetFailReason(document);
        throw new WrongAnswerException(reason);
    }

    private static string GetKey(DateOnly date, int part)
    {
        return $"{date.ToLongDateString()}-{part:00}";
    }
}

public class WrongAnswerException : Exception
{
    public WrongAnswerException()
    {
    }

    public WrongAnswerException(string message) : base(message)
    {
    }

    public WrongAnswerException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}