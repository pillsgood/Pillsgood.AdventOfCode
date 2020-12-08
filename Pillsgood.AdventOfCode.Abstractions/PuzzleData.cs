using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pillsgood.AdventOfCode.Abstractions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class PuzzleData : IPuzzleMetadata
    {
        internal PuzzleData(IPuzzleMetadata metadata)
        {
            Day = metadata.Day;
            Year = metadata.Year;
        }

        [JsonProperty] public int Day { get; }
        [JsonProperty] public int Year { get; }

        [JsonProperty] public string Title { get; internal set; }
        [JsonProperty] public string Input { get; internal set; }
        [JsonProperty] public IEnumerable<PuzzleResult> Results { get; internal set; }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class PuzzleResult
    {
        public string Answer { get; internal set; }

        [JsonProperty] public int Part { get; internal set; }

        [JsonProperty] internal string CorrectAnswer { get; set; }

        [JsonProperty] internal string[] IncorrectAnswers { get; set; }
    }
}