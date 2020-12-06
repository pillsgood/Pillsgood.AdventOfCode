using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class AocLifetimeManager : IDisposable
    {
        internal readonly IContainer container;
        private readonly ILifetimeScope _puzzleScope;
        private PuzzleRunner.Factory _runnerFactory;

        private AocLifetimeManager(IContainer container, PuzzleContainerBuilder puzzleContainerBuilder)
        {
            this.container = container;
            _puzzleScope = container.BeginLifetimeScope(builder =>
            {
                puzzleContainerBuilder.Configure(builder);
                builder.RegisterType<PuzzleRunner>().SingleInstance()
                    .FindConstructorsWith(new AllConstructorFinder());
            });

            _runnerFactory = _puzzleScope.Resolve<PuzzleRunner.Factory>();
        }

        internal static AocLifetimeManager Build(Action<ContainerBuilder> buildServices)
        {
            var containerBuilder = new ContainerBuilder();
            buildServices.Invoke(containerBuilder);
            containerBuilder.RegisterType<Random>().SingleInstance();
            containerBuilder.RegisterType<PuzzleInputRequest.Factory>().SingleInstance();
            containerBuilder.RegisterType<AocLifetimeManager>().SingleInstance()
                .FindConstructorsWith(new AllConstructorFinder());
            containerBuilder.RegisterType<PuzzleContainerBuilder>().AsSelf();
            var container = containerBuilder.Build();
            return container.Resolve<AocLifetimeManager>(new TypedParameter(typeof(IContainer), container));
        }

        internal IPuzzleRunner CreateRunner() => _runnerFactory.Invoke(_puzzleScope);

        internal ILifetimeScope GetPuzzleScope(PuzzleMetadata metadata)
        {
            return _puzzleScope.BeginLifetimeScope(builder =>
            {
                builder.RegisterModule(PuzzleModuleFactory.Create(metadata));
                builder.Register(context => context.Resolve<PuzzleInputRequest.Factory>()
                    .Create(metadata)).SingleInstance();
            });
        }

        public void Dispose()
        {
            container?.Dispose();
            _puzzleScope?.Dispose();
        }
    }
}