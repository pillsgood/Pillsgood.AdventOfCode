namespace Pillsgood.AdventOfCode.Common;

internal static class SessionProviderExtensions
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