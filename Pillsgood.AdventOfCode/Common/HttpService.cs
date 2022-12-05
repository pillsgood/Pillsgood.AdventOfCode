using Splat;

namespace Pillsgood.AdventOfCode.Common;

internal class HttpService
{
    private readonly Lazy<Task<HttpClient>> _httpClientFactory;

    public HttpService()
    {
        _httpClientFactory = new Lazy<Task<HttpClient>>(async () =>
        {
            var httpClient = Locator.Current.GetRequiredService<HttpClient>();
            var sessionProvider = Locator.Current.GetRequiredService<ISessionProvider>();
            var headers = await sessionProvider.GetHeadersAsync();
            foreach (var (key, value) in headers)
            {
                httpClient.DefaultRequestHeaders.Add(key, value);
            }

            return httpClient;
        });
    }

    public async Task<string> GetStringAsync(Uri uri)
    {
        var httpClient = await _httpClientFactory.Value;
        return await httpClient.GetStringAsync(uri);
    }

    public async Task<string> PostAsync(Uri uri, IDictionary<string, string> form)
    {
        var httpClient = await _httpClientFactory.Value;
        var response = await httpClient.PostAsync(uri, new FormUrlEncodedContent(form)).EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}

file static class Extensions
{
    public static async Task<HttpResponseMessage> EnsureSuccessStatusCode(this Task<HttpResponseMessage> task)
    {
        var message = await task;
        return message.EnsureSuccessStatusCode();
    }
}