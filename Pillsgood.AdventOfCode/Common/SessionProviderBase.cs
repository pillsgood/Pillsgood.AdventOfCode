using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Akavache;
using SoftCircuits.HtmlMonkey;

namespace Pillsgood.AdventOfCode.Common;

internal abstract class SessionProviderBase : ISessionProvider
{
    protected virtual IScheduler? Scheduler { get; } = default;
    protected abstract string? GetSession();

    private async Task ValidateSession(string? session)
    {
        if (string.IsNullOrWhiteSpace(session))
        {
            await BlobCache.Secure.Invalidate("session");
        }

        using var httpClient = new HttpClient();

        var page = await httpClient.GetStringAsync("https://adventofcode.com/auth/login");
        if (string.IsNullOrEmpty(page))
        {
            return;
        }

        var document = HtmlDocument.FromHtml(page);
        if (!document.Find("head > title").Any(x => x.Text.Contains("Log In", StringComparison.OrdinalIgnoreCase)))
        {
            await BlobCache.Secure.Invalidate("session");
        }
    }

    public async Task<string> GetSessionAsync()
    {
        try
        {
            var previous = await BlobCache.Secure.GetObject<string>("session");
            await ValidateSession(previous);
        }
        catch (KeyNotFoundException)
        {
        }

        var session = await BlobCache.Secure.GetOrFetchObject("session",
            () => Observable.Defer(() => Observable.Return(GetSession()))
                .SubscribeOn(Scheduler ?? System.Reactive.Concurrency.Scheduler.Default));

        if (string.IsNullOrEmpty(session))
        {
            throw new InvalidOperationException("Session is empty.");
        }


        return session;
    }
}