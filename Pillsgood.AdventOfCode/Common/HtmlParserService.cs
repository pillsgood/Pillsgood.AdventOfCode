using SoftCircuits.HtmlMonkey;

namespace Pillsgood.AdventOfCode.Common;

internal static class HtmlParserService
{
    public static string[] FindAnswers(HtmlDocument document)
    {
        return document.Find("p > code")
            .Where(x => x.ParentNode is { Text: var text } && text.Contains("Your puzzle answer was"))
            .Select(x => x.Text)
            .ToArray();
    }

    public static bool IsAnswerCorrect(HtmlDocument document)
    {
        return document.Find("span[class=\"day-success\"]").Any();
    }

    public static string GetFailReason(HtmlDocument document)
    {
        return document.Find("article > p").First().Text;
    }
}