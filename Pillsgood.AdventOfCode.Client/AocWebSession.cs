using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using Microsoft.Extensions.Logging;
using Pillsgood.AdventOfCode.Abstractions;


namespace Pillsgood.AdventOfCode.Client
{
    internal class AocWebSession : IAocWebSession
    {
        private readonly IPuzzleMetadata _metadata;
        private readonly AocClient _client;
        private readonly ILogger<IAocWebSession> _logger;
        private readonly IBrowsingContext _context;
        private readonly Url _baseUrl;

        public AocWebSession(AocClient client, IPuzzleMetadata metadata)
        {
            _client = client;
            _context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            _baseUrl = new Url(Address.Base);
            _metadata = metadata;
        }

        public AocWebSession(AocClient client, ILogger<IAocWebSession> logger, IPuzzleMetadata metadata) : this(client,
            metadata)
        {
            _logger = logger;
            _logger.LogDebug($"Starting scraper session for {metadata.Year}_{metadata.Day}");
        }

        public Task<string> GetPuzzleInput() => _client.GetPuzzleInput(_metadata);

        public async Task<string> GetDayTitle()
        {
            var address = new Url(_baseUrl, string.Format(Address.Day, _metadata.Year, _metadata.Day));
            var stream = await _client.GetStream(address);
            var document = await _context.OpenAsync(response => response.Content(stream));
            const string selector = "h2";
            var cells = document.QuerySelector(selector);
            return cells.TextContent.Trim(' ', '-');
        }

        private async Task<Dictionary<int, string>> GetAnswers()
        {
            _logger.LogDebug("Scraping webpage for previously answered");
            var address = new Url(_baseUrl, string.Format(Address.Day, _metadata.Year, _metadata.Day));
            var stream = await _client.GetStream(address);
            var document = await _context.OpenAsync(response => response.Content(stream));
            var cells = document.QuerySelectorAll("p")
                .Where(element => element.TextContent.Contains("Your puzzle answer was"));
            return cells.Select(element => element.QuerySelector("code").TextContent)
                .Select((s, i) => new KeyValuePair<int, string>(i, s))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public async Task<string> GetAnswer(int part)
        {
            var answers = await GetAnswers();
            return answers[part];
        }

        public async Task<string> GetAnswerSubmitResult(int part, string answer)
        {
            var address = new Url(_baseUrl, string.Format(Address.PuzzleAnswer, _metadata.Year, _metadata.Day));
            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("level", part.ToString()),
                new KeyValuePair<string, string>("answer", answer)
            });

            var stream = await _client.PostForm(address, form);
            var document = await _context.OpenAsync(response => response.Content(stream));
            return null;
        }
    }
}