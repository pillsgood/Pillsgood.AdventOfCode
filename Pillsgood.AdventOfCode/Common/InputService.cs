using AwesomeAssertions;
using Microsoft.Extensions.Caching.Hybrid;

namespace Pillsgood.AdventOfCode.Common;

internal class InputService : IPuzzleInputService
{
    private readonly HttpService _httpService;
    private readonly HybridCache _cache;

    public InputService(HttpService httpService, HybridCache cache)
    {
        _httpService = httpService;
        _cache = cache;
    }

    public async Task<Stream> GetInputStreamAsync(DateOnly date)
    {
        date.Should().BeAdventDay();

        var uri = UriFactory.GetPuzzleInput(date);

        var data = await _cache.GetOrCreateAsync($"{uri}", async ct => await _httpService.GetAsync(uri, ct));

        return new MemoryStream(data);
    }
}