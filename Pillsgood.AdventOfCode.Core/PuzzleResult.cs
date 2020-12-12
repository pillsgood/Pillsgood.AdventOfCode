using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class PuzzleResult : IPuzzleResult
    {
        private readonly PuzzleDataManager _dataManager;
        private readonly ILogger<PuzzleResult> _logger;

        private readonly Task _populateTask;

        private PuzzleResult()
        {
        }

        internal PuzzleResult(PuzzleData parent, int part, PuzzleDataManager dataManager)
        {
            _dataManager = dataManager;
            Parent = parent;
            Part = part;
            if (!parent.results.Contains(this))
            {
                parent.results = parent.results.Append(this);
            }

            _populateTask = Task.Run(() => dataManager.Populate(this));
        }

        internal PuzzleResult(PuzzleData parent, int part, PuzzleDataManager dataManager,
            ILogger<PuzzleResult> logger) : this(parent, part, dataManager)
        {
            _logger = logger;
            _logger.LogTrace($"Constructing {this}");
        }

        public override string ToString()
        {
            return $"PuzzleResult({Parent.Year}_{Parent.Day}_{Part})";
        }

        internal PuzzleData Parent { get; }
        public string Answer { get; internal set; }
        public Status Status { get; internal set; }

        [JsonProperty] public int Part { get; internal set; }
        [JsonProperty("CorrectAnswer")] internal string correctAnswer;

        public string CorrectAnswer
        {
            get
            {
                _populateTask.Wait();
                return correctAnswer;
            }
        }

        [JsonProperty] public string[] IncorrectAnswers { get; internal set; }
    }

    public enum Status
    {
        Unknown,
        Correct,
        Incorrect,
        IncorrectTooLow,
        IncorrectTooHigh,
        UnknownSubmittedTooRecently,
    }
}