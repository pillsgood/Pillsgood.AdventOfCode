namespace Pillsgood.AdventOfCode.Common;

public interface IAnswerService
{
    Task<string?> GetKnownAnswer(DateOnly date, int part);
    Task SubmitAnswer(DateOnly date, int part, string answer);
}