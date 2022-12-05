namespace Pillsgood.AdventOfCode.Common;

internal static class SessionProviderMixins
{
    public static async ValueTask<IDictionary<string, string>> GetHeadersAsync(this ISessionProvider sessionProvider)
    {
        var session = await sessionProvider.GetSessionAsync();
        return new Dictionary<string, string>()
        {
            ["cookie"] = $"session={session}"
        };
    }
}