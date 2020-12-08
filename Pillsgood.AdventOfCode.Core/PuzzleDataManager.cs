using System;
using System.Collections.Generic;
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
        private readonly IAocConfig _config;
        private readonly IEnumerable<PuzzleData> _puzzleDataSets;
        private readonly IEnumerable<PuzzleMetadata> _puzzleMetadataSets;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public PuzzleDataManager(IEnumerable<Lazy<IPuzzle, PuzzleMetadata>> metaPuzzles,
            IAocScraper scraper,
            IAocConfig config)
        {
            _scraper = scraper;
            _config = config;
            _puzzleMetadataSets = metaPuzzles.Select(lazy => lazy.Metadata);
            _puzzleDataSets = _puzzleMetadataSets.Select(metadata => Deserialize(new PuzzleData(metadata)));
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Reuse
            };
        }

        public IEnumerable<PuzzleMetadata> GetMetadataSets(Func<PuzzleMetadata, bool> predicate) =>
            _puzzleMetadataSets.Where(predicate);

        public Task<PuzzleData> Get(IPuzzleMetadata metadata)
        {
            if (metadata is PuzzleData data)
            {
                return Populate(data);
            }

            data = _puzzleDataSets.First(puzzleData => ((IPuzzleMetadata) puzzleData).Equals(metadata));
            return Populate(data);
        }

        private string GetDataPath(IPuzzleMetadata metadata) =>
            string.Format(_config.SerializationDirectory, metadata.Year, metadata.Day);

        public void Serialize(IPuzzleMetadata metadata)
        {
            var data = Get(metadata).Result;
            var json = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
            var path = GetDataPath(metadata);
            var parent = Directory.GetParent(path);
            if (!parent.Exists)
            {
                parent.Create();
            }

            using var stream = File.CreateText(path);
            stream.Write(json);
        }

        public PuzzleData Deserialize(PuzzleData data)
        {
            if (!File.Exists(GetDataPath(data)))
            {
                return data;
            }

            var path = GetDataPath(data);
            using var stream = File.OpenText(path);
            var json = stream.ReadToEnd();
            JsonConvert.PopulateObject(json, data);
            return data;
        }

        private async Task<PuzzleData> Populate(PuzzleData data)
        {
            if (string.IsNullOrEmpty(data.Title))
            {
                data.Title = await _scraper.GetDayTitle(data);
            }

            await PopulateResult(data);
            return data;
        }

        private async Task PopulateResult(PuzzleData data)
        {
            if (data.Results.Any(result => string.IsNullOrEmpty(result.CorrectAnswer)))
            {
                var answers = (await _scraper.GetAnswer(data)).ToArray();
                if (answers.Length == 0)
                {
                    return;
                }

                var idx = 0;
                foreach (var puzzleResult in data.Results)
                {
                    puzzleResult.CorrectAnswer = answers[idx++];
                }
            }
        }
    }
}