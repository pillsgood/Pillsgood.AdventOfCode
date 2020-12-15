namespace Pillsgood.AdventOfCode.Abstractions
{
    public enum ResultStatus
    {
        Unknown,
        Correct,
        Incorrect,
        IncorrectPreviouslySubmitted,
        IncorrectTooLow,
        IncorrectTooHigh,
        UnknownSubmittedTooRecently,
    }
}