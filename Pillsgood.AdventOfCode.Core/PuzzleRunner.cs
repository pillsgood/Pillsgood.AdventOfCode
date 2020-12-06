using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PuzzleRunner : IPuzzleRunner
    {
        public delegate IPuzzleRunner Factory(ILifetimeScope puzzleScope);

        private readonly PartsHandle.Factory _handleFactory;
        private readonly IEnumerable<Lazy<IPuzzle, PuzzleMetadata>> _puzzles;
        private readonly IAocConsole _console;
        private readonly IServiceProvider _serviceProvider;
        public IServiceProvider ServiceProvider => _serviceProvider;

        internal PuzzleRunner(ILifetimeScope scope,
            IEnumerable<Lazy<IPuzzle, PuzzleMetadata>> puzzles,
            PartsHandle.Factory handleFactory,
            IAocConsole console = null)
        {
            _puzzles = puzzles;
            _handleFactory = handleFactory;
            _console = console;
            _serviceProvider = new AutofacServiceProvider(scope);
        }

        public IEnumerable<KeyValuePair<PuzzleData, IEnumerable<string>>> Run(int? year = null, int? day = null)
        {
            var puzzles = _puzzles.Where(puzzle =>
            {
                var metaData = puzzle.Metadata;
                var valid = true;
                if (year.HasValue)
                {
                    valid = valid && metaData.Year == year;
                }

                if (day.HasValue)
                {
                    valid = valid && metaData.Day == day;
                }

                return valid;
            });

            var result = Run(puzzles);

            if (_console != null)
            {
                result = result.ToArray();
            }

            return result;
        }

        private IEnumerable<KeyValuePair<PuzzleData, IEnumerable<string>>> Run(
            IEnumerable<Lazy<IPuzzle, PuzzleMetadata>> puzzles)
        {
            foreach (var puzzle in puzzles)
            {
                _console?.WriteYear(puzzle.Metadata.Year);
                _console?.WriteDay(puzzle.Metadata.Day);
                var handles = _handleFactory.Invoke(puzzle).Values;
                var answers = RunParts(handles);
                if (_console != null)
                {
                    answers = answers.ToArray();
                }

                yield return
                    new KeyValuePair<PuzzleData, IEnumerable<string>>(new PuzzleData(puzzle.Metadata), answers);
            }

            _console?.PrintSeparator();
        }

        private IEnumerable<string> RunParts(IEnumerable<PartsHandle.ScopeHandle> handles)
        {
            var part = 1;
            foreach (var handle in handles)
            {
                using var scope = handle.Invoke(out var partHandle);
                if (EvaluateAnswer(partHandle, out var answer, ref part)) continue;
                yield return answer;
            }
        }

        private bool EvaluateAnswer(Func<string> handle, out string answer, ref int part)
        {
            answer = null;
            try
            {
                _console?.WritePart(part++);
                answer = handle.Invoke();
                if (answer == null)
                {
                    _console?.WriteAnswerIsNull();
                    return true;
                }

                _console?.WriteAnswer(answer);
            }
            catch (TargetInvocationException e)
            {
                switch (e.InnerException)
                {
                    case NotImplementedException _:
                        _console?.WriteAnswerNotImplemented();
                        return true;
                    case WebException webException:
                        if (_console != null)
                        {
                            _console.WriteNoSessionId();
                            _console.WriteException(webException);
                        }
                        else
                        {
                            throw e.InnerException;
                        }

                        Environment.Exit(1);
                        break;
                }

                if (e.InnerException != null) throw e.InnerException;
                throw;
            }

            return false;
        }
    }
}