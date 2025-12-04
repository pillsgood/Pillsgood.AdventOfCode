namespace Pillsgood.AdventOfCode.Common;

internal class HttpService
{
    private readonly Lazy<Task<HttpClient>> _httpClientFactory;

    public HttpService(HttpClient httpClient, SessionService session)
    {
        _httpClientFactory = new Lazy<Task<HttpClient>>(async () =>
        {
            var headers = await session.GetHeadersAsync();
            foreach (var (key, value) in headers)
            {
                httpClient.DefaultRequestHeaders.Add(key, value);
            }

            return httpClient;
        });
    }

    public async Task<byte[]> GetAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        var httpClient = await _httpClientFactory.Value;
        return await httpClient.GetByteArrayAsync(uri, cancellationToken);
    }

    public async Task<string> GetStringAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        var httpClient = await _httpClientFactory.Value;
        return await httpClient.GetStringAsync(uri, cancellationToken);
    }

    public async Task<string> PostAsync(Uri uri, IDictionary<string, string> form, CancellationToken cancellationToken = default)
    {
        var httpClient = await _httpClientFactory.Value;
        var response = await httpClient.PostAsync(uri, new FormUrlEncodedContent(form), cancellationToken).EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
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