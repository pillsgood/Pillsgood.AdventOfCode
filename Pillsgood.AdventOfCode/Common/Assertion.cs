using FluentAssertions;
using Splat;

namespace Pillsgood.AdventOfCode.Common;

internal class Assertion : IAnswerAssertion
{
    private readonly IAnswerService _answerService;

    public Assertion()
    {
        _answerService = Locator.Current.GetRequiredService<IAnswerService>();
    }

    public async Task Assert(DateOnly date, int part, string answer)
    {
        Console.WriteLine($"You answered: '{answer}'");

        try
        {
            var knownAnswer = await _answerService.GetKnownAnswer(date, part);
            if (string.IsNullOrEmpty(knownAnswer))
            {
                throw new KeyNotFoundException();
            }

            answer.Should().BeEquivalentTo(knownAnswer);
        }
        catch (KeyNotFoundException e)
        {
            await _answerService.SubmitAnswer(date, part, answer);
        }
    }
}