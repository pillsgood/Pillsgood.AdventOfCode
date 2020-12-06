using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.Metadata;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PuzzleRunner : IPuzzleRunner
    {
        public delegate PuzzleRunner Factory(ILifetimeScope puzzleScope);

        private readonly PartsHandle.Factory _handleFactory;
        private readonly IEnumerable<Lazy<IPuzzle, PuzzleMetadata>> _metaPuzzles;
        private readonly IAocConsole _console;
        private readonly IServiceProvider _serviceProvider;
        public IServiceProvider ServiceProvider => _serviceProvider;

        internal PuzzleRunner(ILifetimeScope scope,
            IEnumerable<Lazy<IPuzzle, PuzzleMetadata>> metaPuzzles,
            PartsHandle.Factory handleFactory,
            IAocConsole console = null)
        {
            _metaPuzzles = metaPuzzles;
            _handleFactory = handleFactory;
            _console = console;
            _serviceProvider = new AutofacServiceProvider(scope);
        }

        public IEnumerable<KeyValuePair<PuzzleData, IEnumerable<string>>> Run(int? year = null, int? day = null)
        {
            var metaPuzzles = _metaPuzzles.Where(puzzle =>
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
            }).OrderBy(meta => meta.Metadata.year).ThenBy(meta => meta.Metadata.day);

            var results = Run(metaPuzzles);

            if (_console != null)
            {
                results = results as KeyValuePair<PuzzleData, IEnumerable<string>>[] ?? results.ToArray();
            }

            return results;
        }

        private IEnumerable<KeyValuePair<PuzzleData, IEnumerable<string>>> Run(
            IEnumerable<Lazy<IPuzzle, PuzzleMetadata>> metaPuzzles)
        {
            foreach (var metaPuzzle in metaPuzzles)
            {
                _console?.WriteYear(metaPuzzle.Metadata.Year);
                _console?.WriteDay(metaPuzzle.Metadata.Day);
                var handles = _handleFactory.Invoke(metaPuzzle).Values;
                var answers = RunParts(handles);
                if (_console != null)
                {
                    answers = answers.ToArray();
                }

                yield return
                    new KeyValuePair<PuzzleData, IEnumerable<string>>(new PuzzleData(metaPuzzle.Metadata), answers);
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