using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace Pillsgood.AdventOfCode
{
    public class AdventOfCode : IPuzzleRunner
    {
        private readonly PuzzleRunner _puzzleRunnerImplementation;
        private readonly IAocConsole _console;

        private AdventOfCode(PuzzleRunner runner, IAocConsole console = null)
        {
            _puzzleRunnerImplementation = runner;
            _console = console;
            _console?.StartUpMessage();
        }

        public IEnumerable<IPuzzleData> Run(int? year = null, int? day = null)
        {
            return _console != null
                ? IterateData(_puzzleRunnerImplementation.Run(year, day).Cast<PuzzleData>())
                : _puzzleRunnerImplementation.Run(year, day).Cast<PuzzleData>();
        }

        private IEnumerable<PuzzleData> IterateData(IEnumerable<PuzzleData> puzzleResultsEnumerable)
        {
            var puzzleResults = new List<PuzzleData>();
            foreach (var puzzleData in puzzleResultsEnumerable)
            {
                _console.WriteYear(puzzleData.Year);

                if (string.IsNullOrEmpty(puzzleData.Title))
                {
                    _console.WriteDay(puzzleData.Day);
                }
                else
                {
                    _console.WriteDay(puzzleData.Title);
                }

                IterateResults(puzzleData);
                puzzleResults.Add(puzzleData);
            }

            _console?.PrintSeparator();

            return puzzleResults;
        }

        private void IterateResults(PuzzleData puzzleData)
        {
            puzzleData.results = puzzleData.results.ToArray();
            foreach (var result in puzzleData.results)
            {
                _console.WritePart(result.Part);
                if (result.Answer == null)
                {
                    _console.WriteAnswerIsNull();
                    continue;
                }

                try
                {
                    _console.WriteAnswer(result.Answer);
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
            }
        }

        public object GetService(Type serviceType)
        {
            return _puzzleRunnerImplementation.GetService(serviceType);
        }

        public static AdventOfCode Build(Action<AocConfigBuilder> configure)
        {
            var configBuilder = new AocConfigBuilder();
            configure.Invoke(configBuilder);
            var services = new ServiceCollection();
            configBuilder.config.Services.Invoke(services);
            configBuilder.config.PostConfiguration.Invoke(services);

            return (AdventOfCode) AocLifetimeManager.Build(builder =>
            {
                builder.RegisterInstance(configBuilder.config).As<IAocConfig>().SingleInstance();
                builder.Populate(services);
                
            }, builder => builder.RegisterType<AdventOfCode>().As<IPuzzleRunner>()
                .SingleInstance()
                .FindConstructorsWith(new AllConstructorFinder()));
        }
    }
}