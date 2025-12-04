namespace Pillsgood.AdventOfCode.Common;

internal class StaticSessionProvider : ISessionProvider
{
    private readonly string _session;

    public StaticSessionProvider(string session)
    {
        _session = session;
    }

    public async ValueTask<string?> GetSessionAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_session);
    }
}