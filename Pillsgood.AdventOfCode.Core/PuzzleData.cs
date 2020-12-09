using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class PuzzleData : IPuzzleMetadata, IPuzzleData, IDisposable
    {
        private readonly PuzzleDataManager _dataManager;
        private readonly Task _populateTask;

        [JsonProperty("year")] public int Year { get; private set; }
        [JsonProperty("day")] public int Day { get; private set; }
        [JsonProperty] internal string title;
        [JsonProperty] internal IEnumerable<PuzzleResult> results;
        [JsonProperty] internal string input;


        internal PuzzleData(IPuzzleMetadata metadata, PuzzleDataManager dataManager)
        {
            _dataManager = dataManager;
            Day = metadata.Day;
            Year = metadata.Year;
            results = Enumerable.Range(1, 3).Select(i => new PuzzleResult(i)).ToArray();
            dataManager.Deserialize(this);
            _populateTask = Task.Run(() => dataManager.Populate(this));
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
                return results;
            }
        }

        public void Dispose()
        {
            results = results.Where(result => !result.unused).ToArray();
            _dataManager.Serialize(this);
        }
    }
}