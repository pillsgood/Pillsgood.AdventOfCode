using System;
using Autofac;
using Pillsgood.AdventOfCode.Abstractions;


namespace Pillsgood.AdventOfCode.Core
{
    internal class AocLifetimeManager : IDisposable
    {
        internal readonly IContainer container;
        private readonly ILifetimeScope _lifetimeScope;

        private AocLifetimeManager(IContainer container, PuzzleContainerBuilder puzzleContainerBuilder,
            Action<ContainerBuilder> buildLifetimeServices)
        {
            this.container = container;
            _lifetimeScope = container.BeginLifetimeScope(builder =>
            {
                puzzleContainerBuilder.Configure(builder);
                buildLifetimeServices.Invoke(builder);
                builder.RegisterType<PuzzleRunner>()
                    .WithParameter((info, context) => info.ParameterType == typeof(ILifetimeScope),
                        (info, context) => _lifetimeScope)
                    .FindConstructorsWith(new AllConstructorFinder())
                    .SingleInstance();
                builder.RegisterType<PuzzleDataManager>().SingleInstance();
                builder.RegisterType<PuzzleHandle>()
                    .WithParameter((info, context) => info.ParameterType == typeof(ILifetimeScope),
                        (info, context) => _lifetimeScope)
                    .InstancePerDependency();
            });
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
        }

        private void CurrentDomainOnProcessExit(object sender, EventArgs e)
        {
            Dispose();
        }

        internal static IPuzzleRunner Build(Action<ContainerBuilder> buildRootServices,
            Action<ContainerBuilder> buildLifetimeServices)
        {
            var containerBuilder = new ContainerBuilder();
            buildRootServices.Invoke(containerBuilder);
            containerBuilder.RegisterType<Random>().SingleInstance();
            containerBuilder.RegisterType<AocLifetimeManager>().SingleInstance()
                .FindConstructorsWith(new AllConstructorFinder());
            containerBuilder.RegisterType<PuzzleContainerBuilder>();
            containerBuilder.RegisterType<PuzzleMetadataConfiguration>();
            var container = containerBuilder.Build();
            var lifetimeManager = container.Resolve<AocLifetimeManager>(
                new TypedParameter(typeof(IContainer), container),
                new TypedParameter(typeof(Action<ContainerBuilder>), buildLifetimeServices));
            return lifetimeManager._lifetimeScope.Resolve<IPuzzleRunner>();
        }

        public void Dispose()
        {
            _lifetimeScope?.Dispose();
            container?.Dispose();
        }
    }
}