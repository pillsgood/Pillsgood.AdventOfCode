using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PuzzleDataManager
    {
        private readonly IAocScraper _scraper;
        private readonly IAocClient _client;
        private readonly IAocConfig _config;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public PuzzleDataManager(IAocConfig config)
        {
            _config = config;
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Reuse
            };
        }

        public PuzzleDataManager(IAocConfig config, IAocScraper scraper, IAocClient client) : this(config)
        {
            _scraper = scraper;
            _client = client;
        }


        private string GetDataPath(IPuzzleMetadata metadata) =>
            string.Format(_config.SerializationDirectory, metadata.Year, metadata.Day,
                _client == null ? string.Empty : $"{_client.SessionId.Substring(0, 10)}");

        internal void Serialize(PuzzleData data)
        {
            var json = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
            var path = GetDataPath(data);
            var parent = Directory.GetParent(path);
            if (!parent.Exists)
            {
                parent.Create();
            }

            using var stream = File.CreateText(path);
            stream.Write(json);
        }

        public void Deserialize(PuzzleData data)
        {
            if (!File.Exists(GetDataPath(data)))
            {
                return;
            }

            var path = GetDataPath(data);
            using var stream = File.OpenText(path);
            var json = stream.ReadToEnd();
            JsonConvert.PopulateObject(json, data, _jsonSerializerSettings);
        }

        internal async Task Populate(PuzzleData data)
        {
            if (string.IsNullOrEmpty(data.title))
            {
                data.title = await _scraper.GetDayTitle(data);
            }

            if (string.IsNullOrEmpty(data.input))
            {
                data.input = await _client.GetPuzzleInput(data);
            }

            await PopulateResult(data);
        }

        private async Task PopulateResult(PuzzleData data)
        {
            if (data.results?.Any(result => string.IsNullOrEmpty(result.CorrectAnswer)) == true)
            {
                var answers = (await _scraper.GetAnswer(data)).ToArray();

                if (answers.Length != 0)
                {
                    var idx = 0;
                    foreach (var puzzleResult in data.results)
                    {
                        if (idx < answers.Length)
                        {
                            puzzleResult.CorrectAnswer = answers[idx++];
                        }
                    }
                }
            }
        }
    }
}