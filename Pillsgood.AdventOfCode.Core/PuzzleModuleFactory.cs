using System;
using Autofac.Core;

namespace Pillsgood.AdventOfCode.Core
{
    internal static class PuzzleModuleFactory
    {
        public static IModule Create(PuzzleMetadata metadata)
        {
            var moduleType = typeof(PuzzleModule<>);
            moduleType = moduleType.MakeGenericType(metadata.type);
            return (IModule) Activator.CreateInstance(moduleType);
        }
    }
}