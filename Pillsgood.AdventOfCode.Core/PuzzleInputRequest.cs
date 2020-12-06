using System;
using System.IO;
using System.Net;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    public class PuzzleInputRequest : IPuzzleInput
    {
        private readonly int _year;
        private readonly int _day;

        public PuzzleInputRequest(int year, int day)
        {
            _year = year;
            _day = day;
        }

        private Lazy<string> _value;

        public string Value => _value.Value;


        internal class Factory
        {
            private const string PuzzleInputPath = "CachedPuzzleInput/{0}/{1}.txt";
            private const string RequestUrl = "https://adventofcode.com/{0}/day/{1}/input";
            private readonly string _sessionValue;

            public Factory()
            {
                _sessionValue = $"session={Environment.GetEnvironmentVariable("AOC_SESSION")}";
            }

            public IPuzzleInput Create(IPuzzleInfo puzzleInfo)
            {
                var info = puzzleInfo.ToPuzzleInfo();
                return Create(info.Year, info.Day);
            }

            public IPuzzleInput Create(string year, int day)
            {
                return Create(int.Parse(year), day);
            }

            public IPuzzleInput Create(int year, int day)
            {
                var input = new PuzzleInputRequest(year, day);
                input._value = new Lazy<string>(ValueFactory);
                return input;

                string ValueFactory()
                {
                    if (!LoadFromFile(input, out var value))
                    {
                        value = Request(input);
                        SaveToFile(input, value);
                    }

                    return value;
                }
            }

            private string Request(PuzzleInputRequest input)
            {
                var url = string.Format(RequestUrl, input._year, input._day);
                var request = WebRequest.CreateHttp(url);
                request.Headers["Cookie"] = _sessionValue;

                WebResponse response;
                try
                {
                    response = request.GetResponse();
                }
                catch (WebException)
                {
                    throw new WebException(
                        "AOC Session ID could be missing, ensure you have set AOC_SESSION in your environment variables");
                }

                using var dataStream = response.GetResponseStream();
                if (dataStream == null)
                {
                    throw new NullReferenceException(url);
                }

                using var reader = new StreamReader(dataStream);
                var value = reader.ReadToEnd();
                return value;
            }

            private static void SaveToFile(PuzzleInputRequest input, string value)
            {
                var path = GetCachedPuzzleInputPath(input);
                var parentDir = Directory.GetParent(path);
                if (!parentDir.Exists)
                {
                    parentDir.Create();
                }

                File.WriteAllText(path, value);
            }

            private static string GetCachedPuzzleInputPath(PuzzleInputRequest input) =>
                string.Format(PuzzleInputPath, input._year, input._day);

            private static bool LoadFromFile(PuzzleInputRequest input, out string value)
            {
                value = null;
                var path = GetCachedPuzzleInputPath(input);
                var result = File.Exists(path);
                if (result)
                {
                    value = File.ReadAllText(path);
                }

                return result;
            }
        }
    }
}