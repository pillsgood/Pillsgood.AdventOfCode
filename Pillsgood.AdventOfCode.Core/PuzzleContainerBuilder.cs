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
            AocConfig config)
        {
            foreach (var assembly in config.assemblies)
            {
                var metadataConfiguration = metadataConfigurationFactory.Invoke(assembly);

                foreach (var puzzleType in assembly.GetTypes()
                    .Where(type => type.GetInterfaces().Contains(typeof(IPuzzle))))
                {
                    _builder += builder => builder.RegisterType(puzzleType)
                        .WithMetadata(metadataConfiguration.From(puzzleType));
                }
            }
        }
    }
}