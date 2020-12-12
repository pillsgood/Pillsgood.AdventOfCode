using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Autofac.Features.OwnedInstances;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PuzzleDataManager
    {
        private readonly IAocClient _client;
        private readonly Func<IPuzzleMetadata, Owned<IAocWebSession>> _scraperFactory;
        private readonly ILogger<PuzzleDataManager> _logger;
        private readonly IAocConfig _config;
        private readonly JsonSerializerSettings _jsonSerializerSettings;


        public PuzzleDataManager(IAocConfig config, ILogger<PuzzleDataManager> logger = null)
        {
            _config = config;
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Reuse
            };
            _logger = logger;
        }

        public PuzzleDataManager(IAocConfig config, IAocClient client, Func<IPuzzleMetadata,
            Owned<IAocWebSession>> scraperFactory, ILogger<PuzzleDataManager> logger = null) : this(config, logger)
        {
            _client = client;
            _scraperFactory = scraperFactory;
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

        private readonly Dictionary<IPuzzleMetadata, Owned<IAocWebSession>> _ownedScrapers =
            new Dictionary<IPuzzleMetadata, Owned<IAocWebSession>>();

        private Lazy<IAocWebSession> GetScraperFactory(IPuzzleMetadata metadata) =>
            new Lazy<IAocWebSession>(() =>
            {
                if (!_ownedScrapers.ContainsKey(metadata))
                {
                    _ownedScrapers.Add(metadata, _scraperFactory.Invoke(metadata));
                }

                return _ownedScrapers[metadata].Value;
            });

        internal async Task Populate(PuzzleData data)
        {
            var scraperFactory = GetScraperFactory(data);
            if (string.IsNullOrEmpty(data.title))
            {
                data.title = await scraperFactory.Value.GetDayTitle();
            }

            if (string.IsNullOrEmpty(data.input))
            {
                data.input = await scraperFactory.Value.GetPuzzleInput();
            }
        }

        internal async Task Populate(PuzzleResult result)
        {
            var scraperFactory = GetScraperFactory(result.Parent);
            if (string.IsNullOrEmpty(result.CorrectAnswer))
            {
                result.correctAnswer = await scraperFactory.Value.GetAnswer(result.Part);
            }
        }
    }
}