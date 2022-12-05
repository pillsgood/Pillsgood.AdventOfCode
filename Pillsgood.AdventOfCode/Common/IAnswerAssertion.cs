namespace Pillsgood.AdventOfCode.Common;

public interface IAnswerAssertion
{
    Task Assert(DateOnly date, int part, string answer);
}