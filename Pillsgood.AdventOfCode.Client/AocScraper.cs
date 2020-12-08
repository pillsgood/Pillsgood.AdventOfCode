using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using Pillsgood.AdventOfCode.Abstractions;


namespace Pillsgood.AdventOfCode.Client
{
    internal class AocScraper : IAocScraper
    {
        private readonly AocClient _client;
        private readonly IBrowsingContext _context;
        private readonly Url _baseUrl;

        public AocScraper(AocClient client)
        {
            _client = client;
            _context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            _baseUrl = new Url(Address.Base);
        }

        public async Task<string> GetDayTitle(IPuzzleMetadata metadata)
        {
            var address = new Url(_baseUrl, string.Format(Address.Day, metadata.Year, metadata.Day));
            var stream = await _client.GetStream(address);
            var document = await _context.OpenAsync(response => response.Content(stream));
            const string selector = "h2";
            var cells = document.QuerySelector(selector);
            return cells.TextContent.Trim(' ', '-');
        }

        public async Task<IEnumerable<string>> GetAnswer(IPuzzleMetadata metadata)
        {
            var address = new Url(_baseUrl, string.Format(Address.Day, metadata.Year, metadata.Day));
            var stream = await _client.GetStream(address);
            var document = await _context.OpenAsync(response => response.Content(stream));
            var cells = document.QuerySelectorAll("p")
                .Where(element => element.TextContent.Contains("Your puzzle answer was"));
            return cells.Select(element => element.QuerySelector("code").TextContent);
        }
    }
}