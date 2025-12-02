namespace Pillsgood.AdventOfCode.Common;

internal static class SessionProviderMixins
{
    extension(ISessionProvider sessionProvider)
    {
        public async ValueTask<IDictionary<string, string>> GetHeadersAsync()
        {
            var session = await sessionProvider.GetSessionAsync();
            return new Dictionary<string, string>()
            {
                ["cookie"] = $"session={session}"
            };
        }
    }
}