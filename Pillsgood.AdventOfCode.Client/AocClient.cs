using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Client
{
    internal class AocClient : IAocClient
    {
        private const string BaseAddress = "https://adventofcode.com/";
        private const string PuzzleInputAddress = "{0}/day/{1}/input";

        private readonly HttpClient _httpClient;

        public AocClient(AocClientConfig config)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseAddress) };
            _httpClient.DefaultRequestHeaders.Add("Cookie", $"session={config.SessionId}");
        }

        public async Task<string> GetPuzzleInput(IPuzzleMetadata metadata)
        {
            var address = string.Format(PuzzleInputAddress, metadata.Year, metadata.Day);
            var response = await _httpClient.GetAsync(address);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            throw new WebException();
        }
    }
}