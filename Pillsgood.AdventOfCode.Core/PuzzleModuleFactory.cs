using System;
using Autofac.Core;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PuzzleModuleFactory
    {
        public IModule Create(Type puzzleType)
        {
            var moduleType = typeof(PuzzleModule<>);
            moduleType = moduleType.MakeGenericType(puzzleType);
            return (IModule) Activator.CreateInstance(moduleType);
        }
    }
}