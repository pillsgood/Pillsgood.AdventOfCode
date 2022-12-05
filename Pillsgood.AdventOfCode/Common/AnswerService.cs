using System.Reactive.Linq;
using Akavache;
using FluentAssertions;
using SoftCircuits.HtmlMonkey;
using Splat;

namespace Pillsgood.AdventOfCode.Common;

internal class AnswerService : IAnswerService
{
    private readonly HttpService _httpService;

    public AnswerService()
    {
        _httpService = Locator.Current.GetRequiredService<HttpService>();
    }

    public async Task<string?> GetKnownAnswer(DateOnly date, int part)
    {
        try
        {
            return await BlobCache.LocalMachine.GetOrFetchObject(GetKey(date, part), FetchFunc);
        }
        catch (KeyNotFoundException)
        {
            return string.Empty;
        }

        async Task<string> FetchFunc()
        {
            var uri = UriFactory.GetPuzzle(date);
            var html = await _httpService.GetStringAsync(uri);
            var document = HtmlDocument.FromHtml(html);
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
        var previous = await BlobCache.LocalMachine.GetObject<string>(key).Catch(Observable.Return(string.Empty));

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

        var document = HtmlDocument.FromHtml(html);
        if (HtmlParserService.IsAnswerCorrect(document))
        {
            await BlobCache.LocalMachine.InsertObject(key, answer);
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