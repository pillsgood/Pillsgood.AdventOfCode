namespace Pillsgood.AdventOfCode.Common;

public interface ISessionProvider
{
    Task<string> GetSessionAsync(CancellationToken cancellationToken = default);
}