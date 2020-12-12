using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class PuzzleData : IPuzzleMetadata, IPuzzleData, IDisposable
    {
        private readonly PuzzleDataManager _dataManager;
        private readonly Func<PuzzleData, int, PuzzleResult> _resultFactory;
        private readonly ILogger<PuzzleData> _logger;
        private readonly Task _populateTask;

        [JsonProperty("year")] public int Year { get; private set; }
        [JsonProperty("day")] public int Day { get; private set; }
        [JsonProperty] internal string title;
        [JsonProperty] internal IEnumerable<PuzzleResult> results = new PuzzleResult[0];
        [JsonProperty] internal string input;


        internal PuzzleData(IPuzzleMetadata metadata, PuzzleDataManager dataManager,
            Func<PuzzleData, int, PuzzleResult> resultFactory)
        {
            _dataManager = dataManager;
            _resultFactory = resultFactory;
            Day = metadata.Day;
            Year = metadata.Year;
            dataManager.Deserialize(this);
            _populateTask = Task.Run(() => dataManager.Populate(this));
        }

        internal PuzzleData(IPuzzleMetadata metadata, PuzzleDataManager dataManager,
            Func<PuzzleData, int, PuzzleResult> resultFactory,
            ILogger<PuzzleData> logger) : this(metadata, dataManager, resultFactory)
        {
            _logger = logger;
            _logger.LogTrace($"Constructing {this}");
        }

        public override string ToString()
        {
            return $"PuzzleData({Year}_{Day})";
        }

        internal PuzzleResult GetResult(int part)
        {
            var result = results.FirstOrDefault(r => r.Part == part);
            if (result == null)
            {
                result = _resultFactory.Invoke(this, part);
            }

            return result;
        }


        public string Title
        {
            get
            {
                _populateTask.Wait();
                return title;
            }
        }

        public string Input
        {
            get
            {
                _populateTask.Wait();
                return input;
            }
        }

        public IEnumerable<IPuzzleResult> Results
        {
            get
            {
                _populateTask.Wait();
                return results.OrderBy(result => result.Part);
            }
        }

        public void Dispose()
        {
            _dataManager.Serialize(this);
        }
    }
}