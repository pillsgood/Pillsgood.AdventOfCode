using AwesomeAssertions;
using Microsoft.Extensions.Caching.Hybrid;
using SoftCircuits.HtmlMonkey;

namespace Pillsgood.AdventOfCode.Common;

internal sealed class SessionService
{
    private readonly HybridCache _cache;
    private readonly ISessionProvider _sessionProvider;

    public SessionService(HybridCache cache, ISessionProvider sessionProvider)
    {
        _cache = cache;
        _sessionProvider = sessionProvider;
    }

    private async Task ValidateSession(string? session)
    {
        if (string.IsNullOrWhiteSpace(session))
        {
            await _cache.RemoveAsync("session");
        }

        using var httpClient = new HttpClient();

        var page = await httpClient.GetStringAsync("https://adventofcode.com/auth/login");
        if (string.IsNullOrEmpty(page))
        {
            return;
        }

        var document = await HtmlDocument.FromHtmlAsync(page);
        if (!document.Find("head > title").Any(x => x.Text.Contains("Log In", StringComparison.OrdinalIgnoreCase)))
        {
            await _cache.RemoveAsync("session");
        }
    }

    public async Task<string> GetSessionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var previous = await _cache.GetAsync<string>("session", cancellationToken: cancellationToken);
            await ValidateSession(previous);
        }
        catch (KeyNotFoundException)
        {
        }

        var session = await _cache.GetOrCreateAsync("session", _sessionProvider.GetSessionAsync, cancellationToken: cancellationToken);

        session.Should().NotBeNullOrWhiteSpace();

        return session;
    }
}