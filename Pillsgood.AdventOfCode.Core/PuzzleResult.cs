using System;
using Newtonsoft.Json;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class PuzzleResult : IPuzzleResult
    {
        internal PuzzleResult()
        {
        }

        internal PuzzleResult(int part)
        {
            Part = part;
            unused = true;
        }

        public string Answer { get; internal set; }

        internal bool unused;

        [JsonProperty] public int Part { get; internal set; }

        [JsonProperty] public string CorrectAnswer { get; internal set; }

        [JsonProperty] public string[] IncorrectAnswers { get; internal set; }
    }
}