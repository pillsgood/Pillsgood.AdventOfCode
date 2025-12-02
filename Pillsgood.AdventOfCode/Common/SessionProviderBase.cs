using AwesomeAssertions;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using SoftCircuits.HtmlMonkey;

namespace Pillsgood.AdventOfCode.Common;

internal abstract class SessionProviderBase : ISessionProvider
{
    private readonly HybridCache _cache = Locator.Current.GetRequiredService<HybridCache>();

    protected abstract string? GetSession();

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

        var session = await _cache.GetOrCreateAsync("session", _ => ValueTask.FromResult(GetSession()), cancellationToken: cancellationToken);

        session.Should().NotBeNullOrWhiteSpace();

        return session;
    }
}