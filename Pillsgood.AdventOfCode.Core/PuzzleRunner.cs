using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    public class PuzzleRunner : IPuzzleRunner
    {
        private readonly AocLifetimeManager _aocLifetimeManager;
        private readonly IAocConsole _console;
        private readonly IServiceProvider _serviceProvider;
        public IServiceProvider ServiceProvider => _serviceProvider;

        internal PuzzleRunner(AocLifetimeManager aocLifetimeManager, IAocConsole console = null)
        {
            _aocLifetimeManager = aocLifetimeManager;
            _console = console;
            _serviceProvider = new AutofacServiceProvider(_aocLifetimeManager.container);
        }

        public IEnumerable<KeyValuePair<PuzzleInfo, string[]>> Run(int? year = null, int? day = null)
        {
            var puzzles = _aocLifetimeManager.RegisteredPuzzles.Where(identification =>
            {
                var valid = true;
                if (year.HasValue)
                {
                    valid = valid && identification.Year == year;
                }

                if (day.HasValue)
                {
                    valid = valid && identification.Day == day;
                }

                return valid;
            }).ToArray();

            return Run(puzzles).ToArray();
        }

        private IEnumerable<KeyValuePair<PuzzleInfo, string[]>> Run(
            params IPuzzleInfo[] puzzleInfos)
        {
            foreach (var puzzleInfo in puzzleInfos)
            {
                var info = puzzleInfo.ToPuzzleInfo();
                _console?.WriteYear(info.Year);
                _console?.WriteDay(info.Day);
                var partsHandle = _aocLifetimeManager.GetSolveHandle(puzzleInfo);
                var part = 1;
                var answers = new List<string>();
                foreach (var handle in partsHandle)
                {
                    _console?.WritePart(part++);

                    try
                    {
                        var answer = handle.Invoke();
                        if (answer == null)
                        {
                            _console?.WriteAnswerIsNull();
                            continue;
                        }

                        _console?.WriteAnswer(answer);
                        answers.Add(answer);
                    }
                    catch (TargetInvocationException e)
                    {
                        switch (e.InnerException)
                        {
                            case NotImplementedException _:
                                _console?.WriteAnswerNotImplemented();
                                continue;
                            case WebException webException:
                                if (_console != null)
                                {
                                    _console?.WriteNoSessionId();
                                    _console?.WriteException(webException);
                                }
                                else
                                {
                                    throw;
                                }

                                Environment.Exit(-1);
                                break;
                        }


                        if (e.InnerException != null) throw e.InnerException;
                        throw;
                    }
                }

                yield return new KeyValuePair<PuzzleInfo, string[]>(info, answers.ToArray());
            }

            _console?.PrintSeparator();
        }
    }
}