using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PuzzleRunner : IPuzzleRunner
    {
        public delegate PuzzleRunner Factory(ILifetimeScope puzzleScope);

        private readonly AocLifetimeManager _lifetimeManager;
        private readonly PuzzleDataManager _dataManager;
        private readonly IAocConsole _console;
        private readonly IServiceProvider _serviceProvider;
        public IServiceProvider ServiceProvider => _serviceProvider;

        internal PuzzleRunner(ILifetimeScope scope,
            AocLifetimeManager lifetimeManager,
            PuzzleDataManager dataManager,
            IAocConsole console = null)
        {
            _lifetimeManager = lifetimeManager;
            _dataManager = dataManager;
            _console = console;
            _serviceProvider = new AutofacServiceProvider(scope);
        }

        public IEnumerable<PuzzleData> Run(int? year = null, int? day = null)
        {
            var metadataSets = _dataManager.GetMetadataSets(metadata =>
            {
                var valid = true;
                if (year.HasValue)
                {
                    valid = valid && metadata.Year == year;
                }

                if (day.HasValue)
                {
                    valid = valid && metadata.Day == day;
                }

                return valid;
            }).OrderBy(metadata => metadata.Year).ThenBy(metadata => metadata.Day);


            var results = Run(metadataSets);

            if (_console != null)
            {
                results = results as PuzzleData[] ?? results.ToArray();
            }

            return results;
        }

        private IEnumerable<PuzzleData> Run(
            IEnumerable<PuzzleMetadata> metadataSets)
        {
            foreach (var metadata in metadataSets)
            {
                var data = _dataManager.Get(metadata).Result;
                _console?.WriteYear(metadata.Year);
                
                if (string.IsNullOrEmpty(data.Title))
                {
                    _console?.WriteDay(data.Day);
                }
                else
                {
                    _console?.WriteDay(data.Title);
                }

                using var dayScope = _lifetimeManager.StartPuzzleScope(metadata, out var factory);
                var handles = factory.Invoke(metadata, dayScope);
                var results = RunParts(handles.Values);
                if (_console != null)
                {
                    results = results.ToArray();
                }
                
                yield return data;

                _dataManager.Serialize(data);
            }

            _console?.PrintSeparator();
        }

        private IEnumerable<PuzzleResult> RunParts(IEnumerable<PartsHandle.ScopeHandle> handles)
        {
            foreach (var handle in handles)
            {
                using var scope = handle.Invoke(out var partHandle, out var part);
                if (EvaluateAnswer(partHandle, out var answer, ref part)) continue;
                yield return new PuzzleResult
                {
                    Answer = answer, Part = part
                };
            }
        }

        private bool EvaluateAnswer(Func<string> handle, out string answer, ref int part)
        {
            answer = null;
            try
            {
                _console?.WritePart(part);
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

                if (e.InnerException != null) throw;
                throw;
            }

            return false;
        }
    }
}