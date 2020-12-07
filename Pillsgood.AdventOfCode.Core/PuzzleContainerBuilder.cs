using System;
using System.Linq;
using Autofac;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PuzzleContainerBuilder
    {
        private readonly Action<ContainerBuilder> _builder = delegate { };
        public void Configure(ContainerBuilder containerBuilder) => _builder.Invoke(containerBuilder);

        public PuzzleContainerBuilder(PuzzleMetadataConfiguration.Factory metadataConfigurationFactory,
            IAocConfig config)
        {
            foreach (var assembly in config.Assemblies)
            {
                var metadataConfiguration = metadataConfigurationFactory.Invoke(assembly);

                foreach (var puzzleType in assembly.GetTypes()
                    .Where(type => type.GetInterfaces().Contains(typeof(IPuzzle))))
                {
                    _builder += builder => builder.RegisterType(puzzleType).As<IPuzzle>()
                        .WithMetadata(metadataConfiguration.From(puzzleType, out var metadata))
                        .Keyed<IPuzzle>(metadata).InstancePerLifetimeScope();
                }
            }
        }
    }
}