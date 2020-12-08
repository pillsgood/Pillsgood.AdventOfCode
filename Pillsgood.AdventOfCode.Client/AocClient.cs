using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using Pillsgood.AdventOfCode.Abstractions;


namespace Pillsgood.AdventOfCode.Client
{
    internal class AocClient : IAocClient
    {
        private const string PuzzleInputAddress = Address.Base + Address.Day + "/input";

        private readonly HttpClient _httpClient;

        public AocClient(AocClientConfig config)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(Address.Base) };
            _httpClient.DefaultRequestHeaders.Add("Cookie", $"session={config.SessionId}");
        }

        public async Task<string> GetPuzzleInput(IPuzzleMetadata metadata)
        {
            var address = string.Format(PuzzleInputAddress, metadata.Year, metadata.Day);
            var response = await _httpClient.GetAsync(address);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        internal async Task<Stream> GetStream(Url uri)
        {
            return await _httpClient.GetStreamAsync(uri);
        }
    }
}