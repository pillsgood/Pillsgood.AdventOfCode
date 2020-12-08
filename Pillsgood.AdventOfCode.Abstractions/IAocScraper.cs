using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pillsgood.AdventOfCode.Abstractions
{
    internal interface IAocScraper
    {
        Task<string> GetDayTitle(IPuzzleMetadata metadata);
        Task<IEnumerable<string>> GetAnswer(IPuzzleMetadata metadata);
    }
}