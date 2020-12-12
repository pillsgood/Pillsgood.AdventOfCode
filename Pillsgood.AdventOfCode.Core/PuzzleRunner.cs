using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PuzzleRunner : IPuzzleRunner
    {
        public event Action<(IPuzzleMetadata, int), Exception> PuzzleExceptionThrown;

        private readonly PuzzleHandle.Factory _puzzleHandleFactory;
        private readonly IEnumerable<Lazy<PuzzleData, PuzzleMetadata>> _puzzleDataSets;
        private readonly IServiceProvider _serviceProvider;

        internal PuzzleRunner(ILifetimeScope scope,
            PuzzleHandle.Factory puzzleHandleFactory,
            IEnumerable<Lazy<PuzzleData, PuzzleMetadata>> puzzleDataSets)
        {
            _puzzleHandleFactory = puzzleHandleFactory;
            _puzzleDataSets = puzzleDataSets;
            _serviceProvider = new AutofacServiceProvider(scope);
        }

        public IEnumerable<IPuzzleData> Run(int? year = null, int? day = null)
        {
            var metadataSets = _puzzleDataSets
                .Where(dataSet => (!year.HasValue || year.Value == dataSet.Metadata.Year) &&
                                  (!day.HasValue || day.Value == dataSet.Metadata.Day))
                .OrderBy(dataSet => dataSet.Metadata.Year)
                .ThenBy(dataSet => dataSet.Metadata.Day);
            return Run(metadataSets);
        }

        private IEnumerable<PuzzleData> Run(IEnumerable<Lazy<PuzzleData, PuzzleMetadata>> dataSets)
        {
            foreach (var dataSet in dataSets)
            {
                var data = dataSet.Value;
                RunParts(dataSet.Metadata, data);
                yield return data;
            }
        }

        private void RunParts(PuzzleMetadata metadata, PuzzleData puzzleData)
        {
            using var handles = _puzzleHandleFactory.Invoke(metadata);
            foreach (var (key, scopeHandle) in handles.Values.OrderBy(pair => pair.Key))
            {
                using var scope = scopeHandle.Invoke(out var handle);
                var result = puzzleData.GetResult(key);
                try
                {
                    if (GetAnswer(handle, out var answer))
                    {
                        result.Answer = answer;
                    }
                    else
                    {
                        result.Answer = null;
                    }
                }
                catch (Exception e)
                {
                    PuzzleExceptionThrown?.Invoke((metadata, key), e);
                }
            }
        }

        private bool EvaluateAnswer(PuzzleMetadata metadata, PuzzleResult result)
        {
            // var response = _scraper.GetAnswerSubmitResult(metadata, result.Part, result.Answer).Result;
            return false;
        }

        private bool GetAnswer(Func<string> handle, out string answer)
        {
            answer = handle.Invoke();
            if (string.IsNullOrEmpty(answer))
            {
                throw new PuzzleNullOrEmptyException();
            }

            return answer != null;
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }
    }
}