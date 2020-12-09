namespace Pillsgood.AdventOfCode.Abstractions
{
    public interface IPuzzleResult
    {
        string Answer { get; }
        int Part { get; }
        string CorrectAnswer { get; }
        string[] IncorrectAnswers { get; }
    }
}