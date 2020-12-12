using System.Threading.Tasks;

namespace Pillsgood.AdventOfCode.Abstractions
{
    internal interface IAocWebSession
    {
        Task<string> GetPuzzleInput();
        Task<string> GetDayTitle();
        Task<string> GetAnswer(int part);
        Task<string> GetAnswerSubmitResult(int part, string answer);
    }
}