using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PuzzleDataManager
    {
        private readonly IAocConfig _config;
        private readonly IEnumerable<PuzzleData> _puzzleDataSets;
        private readonly IEnumerable<PuzzleMetadata> _puzzleMetadataSets;
        private readonly List<PuzzleData> _uninitializedDataSets;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public PuzzleDataManager(IEnumerable<Lazy<IPuzzle, PuzzleMetadata>> metaPuzzles, IAocConfig config)
        {
            _config = config;
            _puzzleMetadataSets = metaPuzzles.Select(lazy => lazy.Metadata);
            _puzzleDataSets = _puzzleMetadataSets.Select(metadata => new PuzzleData(metadata)).ToArray();
            _uninitializedDataSets = _puzzleDataSets.ToList();
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

        public PuzzleData Get(IPuzzleMetadata metadata)
        {
            if (_puzzleDataSets.Contains(metadata))
            {
                return (PuzzleData) metadata;
            }

            var data = _puzzleDataSets.First(puzzleData => ((IPuzzleMetadata) puzzleData).Equals(metadata));
            if (File.Exists(GetDataPath(data)) && _uninitializedDataSets.Contains(data))
            {
                Deserialize(data);
                _uninitializedDataSets.Remove(data);
            }

            return data;
        }

        private string GetDataPath(IPuzzleMetadata metadata) =>
            string.Format(_config.SerializationDirectory, metadata.Year, metadata.Day);

        public void Serialize(IPuzzleMetadata metadata)
        {
            var data = Get(metadata);
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

        public void Deserialize(IPuzzleMetadata metadata)
        {
            var data = Get(metadata);
            var path = GetDataPath(metadata);
            using var stream = File.OpenText(path);
            var json = stream.ReadToEnd();
            JsonConvert.PopulateObject(json, data);
            _uninitializedDataSets.Remove(data);
        }
    }
}