#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.Extensions.Logging;

namespace Pillsgood.AdventOfCode.Console
{
    internal class AocConsole : IAocConsole
    {
        private static readonly string[] YearFormats;

        static AocConsole()
        {
            YearFormats = LoadYearFormats();
        }

        private static string[] LoadYearFormats()
        {
            var yearFormats = new List<string>();
            string? currentYearFormat;
            var idx = 0;
            do
            {
                currentYearFormat = AocResource.ResourceManager.GetString($"YearFormat{idx++}");
                if (currentYearFormat != null) yearFormats.Add(currentYearFormat);
            } while (currentYearFormat != null);

            return yearFormats.ToArray();
        }

        private readonly IConsoleWriter _console;
        private readonly IAocConfig _aocConfig;
        private readonly AocConsoleConfig _consoleConfig;
        private readonly Random _random;

        private readonly Lazy<string> _separator;

        private int currentYear = 0;
        private int Width => (int) (_consoleConfig.Width ?? 64);

        public AocConsole(IConsoleWriter console, IAocConfig aocConfig, AocConsoleConfig consoleConfig, Random random)
        {
            _console = console;
            _aocConfig = aocConfig;
            _consoleConfig = consoleConfig;
            _random = random;
            _separator = new Lazy<string>(() => new string(Enumerable.Repeat('-', Width).ToArray()));
            System.Console.WindowWidth = Width + 2;
        }

        public void PrintSeparator()
        {
            _console.WriteLine(message: s => s.Append(_separator.Value).Color(Color.DarkGreen));
        }


        public void StartUpMessage()
        {
            PrintSeparator();
            _console.WriteLine(s => s.Append(AocResource.Title.PadLeft(Width)).Color(ConsoleColor.Green));
            if (_aocConfig.Year.HasValue)
            {
                WriteYear(_aocConfig.Year.Value);
            }
        }

        public void WriteYear(int year)
        {
            if (currentYear == year)
            {
                return;
            }

            var yearStr = year.ToString();
            var ansiYear = AnsiString.Build(s => s.Append(year).Color(Color.Lime));
            var align = Width + (ansiYear.Length - yearStr.Length);
            var message = string.Format(YearFormats[_random.Next(YearFormats.Length)], ansiYear);
            _console.WriteLine(s => s.Append(message.PadLeft(align)).Color(Color.DarkGreen));
            currentYear = year;
        }

        public void WritePart(int part)
        {
            WriteHeader($"Part {part}", AocResource.Header, Color.White, Color.Gray);
        }

        public void WriteDay(int day)
        {
            WriteHeader($"Day {day}", AocResource.HeaderLong, Color.Lime, Color.Green);
        }

        public void WriteAnswer(string answer)
        {
            _console.WriteLine();
            _console.WriteLine(s =>
                s.Append("Your puzzle answer is ").Color(Color.LightGray).Append($" {answer} ").Color(Color.Gold));
            _console.WriteLine();
        }

        public void WriteException(Exception e)
        {
            _console.Write(s => s.AppendLine(e.Message).Color(Color.Red).AppendLine(e.StackTrace).Color(Color.Gray));
        }

        public void WriteNoSessionId()
        {
            _console.WriteLine(message: s => s
                .Append("AOC Session ID could be missing, ensure you have set ").Color(Color.OrangeRed)
                .Append("AOC_SESSION").Color(Color.Yellow)
                .Append(" environment variable").Color(Color.OrangeRed));
        }

        public void WriteAnswerNotImplemented()
        {
            _console.WriteLine();
            _console.WriteLine(s =>
                s.Append("To begin use ").Color(Color.Gray).Append("IPuzzleInput ").Color(Color.DodgerBlue)
                    .Append("to get your puzzle input").Color(Color.Gray));
            _console.WriteLine();
        }

        public void WriteAnswerIsNull()
        {
            _console.WriteLine();
            _console.WriteLine(s => s.Append("No Answer Found").Color(Color.OrangeRed));
            _console.WriteLine();
        }

        private void WriteHeader(string title, string border, Color color, Color borderColor)
        {
            var ansiTitle = AnsiString.Build(s => s.Append(title).Color(color));
            border += new string(Enumerable.Repeat('-', Width - border.Length - title.Length).ToArray());
            _console.WriteLine(s => s.AppendFormat(border, ansiTitle).Color(borderColor));
        }
    }
}