namespace Pillsgood.AdventOfCode.Common;

public interface ISessionProvider
{
    ValueTask<string?> GetSessionAsync(CancellationToken cancellationToken = default);
}