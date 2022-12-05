using System.Reactive.Linq;
using Akavache;
using FluentAssertions;
using Splat;

namespace Pillsgood.AdventOfCode.Common;

internal class InputService : IPuzzleInputService
{
    private readonly ISessionProvider _sessionProvider;

    public InputService()
    {
        _sessionProvider = Locator.Current.GetRequiredService<ISessionProvider>();
    }

    public async Task<Stream> GetInputStreamAsync(DateOnly date)
    {
        date.Should().BeAdventDay();

        var uri = UriFactory.GetPuzzleInput(date);

        var headers = await _sessionProvider.GetHeadersAsync();
        var data = await BlobCache.LocalMachine.DownloadUrl(uri, headers);

        return new MemoryStream(data);
    }
}