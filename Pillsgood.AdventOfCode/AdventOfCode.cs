using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace Pillsgood.AdventOfCode
{
    public class AdventOfCode : IPuzzleRunner
    {
        private readonly IPuzzleRunner _puzzleRunnerImplementation;

        private AdventOfCode(AocLifetimeManager lifetimeManager, IAocConsole aocConsole = null)
        {
            _puzzleRunnerImplementation = lifetimeManager.ResolveRunner();
            aocConsole?.StartUpMessage();
        }

        public IEnumerable<PuzzleData> Run(int? year = null, int? day = null)
        {
            return _puzzleRunnerImplementation.Run(year, day);
        }

        public IServiceProvider ServiceProvider => _puzzleRunnerImplementation.ServiceProvider;

        public static AdventOfCode Build(Action<AocConfigBuilder> configure)
        {
            var configBuilder = new AocConfigBuilder();
            configure.Invoke(configBuilder);
            var services = new ServiceCollection();
            configBuilder.config.Services.Invoke(services);

            var lifetimeManager = AocLifetimeManager.Build(builder =>
            {
                builder.RegisterInstance(configBuilder.config).As<IAocConfig>().SingleInstance();
                builder.RegisterType<AdventOfCode>().SingleInstance().FindConstructorsWith(new AllConstructorFinder());
                builder.Populate(services);
            });

            return lifetimeManager.container.Resolve<AdventOfCode>();
        }
    }
}