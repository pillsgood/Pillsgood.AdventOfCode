using System;
using Autofac;
using Pillsgood.AdventOfCode.Abstractions;


namespace Pillsgood.AdventOfCode.Core
{
    internal class AocLifetimeManager : IDisposable
    {
        internal readonly IContainer container;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly PuzzleRunner.Factory _runnerFactory;

        private AocLifetimeManager(IContainer container, PuzzleContainerBuilder puzzleContainerBuilder)
        {
            this.container = container;
            _lifetimeScope = container.BeginLifetimeScope(builder =>
            {
                puzzleContainerBuilder.Configure(builder);
                builder.RegisterType<PuzzleRunner>()
                    .FindConstructorsWith(new AllConstructorFinder());
                builder.RegisterType<PuzzleDataManager>().SingleInstance();
                builder.RegisterType<PuzzleInputFactory>().SingleInstance();
            });
            _runnerFactory = _lifetimeScope.Resolve<PuzzleRunner.Factory>();
        }

        public IPuzzleRunner ResolveRunner() => _runnerFactory.Invoke(_lifetimeScope);

        internal static AocLifetimeManager Build(Action<ContainerBuilder> buildServices)
        {
            var containerBuilder = new ContainerBuilder();
            buildServices.Invoke(containerBuilder);
            containerBuilder.RegisterType<Random>().SingleInstance();
            containerBuilder.RegisterType<AocLifetimeManager>().SingleInstance()
                .FindConstructorsWith(new AllConstructorFinder());
            containerBuilder.RegisterType<PuzzleContainerBuilder>();
            containerBuilder.RegisterType<PuzzleMetadataConfiguration>();
            containerBuilder.RegisterType<PartsHandle>();
            var container = containerBuilder.Build();
            return container.Resolve<AocLifetimeManager>(new TypedParameter(typeof(IContainer), container));
        }

        internal ILifetimeScope StartPuzzleScope(PuzzleMetadata metadata, out PartsHandle.Factory partHandleFactory)
        {
            var scope = _lifetimeScope.BeginLifetimeScope(builder =>
            {
                builder.RegisterModule(PuzzleModuleFactory.Create(metadata));
                builder.Register(context => context.Resolve<PuzzleInputFactory>()
                    .Create(metadata)).As<IPuzzleInput>().SingleInstance();
            });
            partHandleFactory = scope.Resolve<PartsHandle.Factory>();
            return scope;
        }

        public void Dispose()
        {
            container?.Dispose();
            _lifetimeScope?.Dispose();
        }
    }
}