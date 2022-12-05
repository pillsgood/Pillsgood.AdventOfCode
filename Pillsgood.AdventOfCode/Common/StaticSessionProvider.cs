namespace Pillsgood.AdventOfCode.Common;

internal class StaticSessionProvider : ISessionProvider
{
    private readonly string _session;

    public StaticSessionProvider(string session)
    {
        _session = session;
    }

    public Task<string> GetSessionAsync()
    {
        return Task.FromResult(_session);
    }
}