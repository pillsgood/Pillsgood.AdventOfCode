using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class AocLifetimeManager : IDisposable
    {
        internal readonly IContainer container;
        public IPuzzleInfo[] RegisteredPuzzles { get; private set; }

        private readonly Dictionary<IPuzzleInfo, IModule> _puzzleModules =
            new Dictionary<IPuzzleInfo, IModule>();

        private ILifetimeScope _puzzleScope;

        private AocLifetimeManager(IContainer container)
        {
            this.container = container;
        }

        internal static AocLifetimeManager Build(Action<ContainerBuilder> buildServices)
        {
            var containerBuilder = new ContainerBuilder();
            buildServices.Invoke(containerBuilder);
            containerBuilder.RegisterType<Random>().SingleInstance();
            containerBuilder.RegisterType<PuzzleInputRequest.Factory>().SingleInstance();
            containerBuilder.RegisterType<AocLifetimeManager>().SingleInstance()
                .FindConstructorsWith(new AllConstructorFinder());
            containerBuilder.RegisterType<PuzzleRunner>().SingleInstance()
                .FindConstructorsWith(new AllConstructorFinder());
            var container = containerBuilder.Build();
            return container.Resolve<AocLifetimeManager>(new TypedParameter(typeof(IContainer), container));
        }

        internal PuzzleRunner CreateRunner()
        {
            return container.Resolve<PuzzleRunner>();
        }

        public IDisposable BeginPuzzleRegistration(out Action<Type, IPuzzleInfo> registerPuzzleType)
        {
            var builder = new PuzzleContainerBuilder(this);
            registerPuzzleType = builder.AddPuzzle;
            return builder;
        }

        internal IEnumerable<Func<string>> GetSolveHandle(IPuzzleInfo puzzleInfo)
        {
            using var scope = _puzzleScope.BeginLifetimeScope(builder =>
            {
                builder.RegisterModule(_puzzleModules[puzzleInfo]);
                builder.Register(context => context.Resolve<PuzzleInputRequest.Factory>().Create(puzzleInfo))
                    .SingleInstance();
            });
            var puzzle = scope.ResolveKeyed<IPuzzle>(puzzleInfo);
            var parts = GetAllParts(puzzle);

            foreach (var (value, methodInfo) in parts)
            {
                using var partScope = scope.BeginLifetimeScope();
                var partIdentification = puzzleInfo as IPuzzlePartInfo;
                if (partIdentification?.Part.HasValue == true
                    && value != partIdentification.Part.Value)
                {
                    continue;
                }

                using (InjectParameters(partScope, puzzle, methodInfo, out var handle))
                {
                    yield return handle;
                }
            }
        }

        private IDisposable InjectParameters(ILifetimeScope scope, IPuzzle puzzle, MethodInfo methodInfo,
            out Func<string> handle)
        {
            if (methodInfo.ReturnType != typeof(string))
            {
                throw new Exception($"{puzzle.GetType().Name}.{methodInfo.Name}() requires string return type");
            }

            var parameterInfos = methodInfo.GetParameters().ToList();
            parameterInfos.Sort((infoA, infoB) => infoA.Position.CompareTo(infoB.Position));
            var parameters = parameterInfos.Select(info => info.HasDefaultValue
                ? scope.ResolveOptional(info.ParameterType) ?? info.DefaultValue
                : scope.Resolve(info.ParameterType)).ToList();
            handle = () => (string) methodInfo.Invoke(puzzle, parameters.ToArray());
            return scope;
        }

        private static SortedDictionary<int, MethodInfo> GetAllParts(IPuzzle puzzle)
        {
            var parts = new SortedDictionary<int, MethodInfo>();
            foreach (var methodInfo in puzzle.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var partAttr = methodInfo.GetCustomAttribute<PartAttribute>();
                if (partAttr == null)
                {
                    continue;
                }

                parts.Add(partAttr.Value, methodInfo);
            }

            return parts;
        }

        public void Dispose()
        {
            container?.Dispose();
            _puzzleScope?.Dispose();
        }

        private class PuzzleContainerBuilder : IDisposable
        {
            private readonly AocLifetimeManager _aocLifetimeManager;

            private readonly Dictionary<Type, IPuzzleInfo> _puzzles =
                new Dictionary<Type, IPuzzleInfo>();

            private readonly PuzzleModuleFactory _moduleFactory = new PuzzleModuleFactory();

            public PuzzleContainerBuilder(AocLifetimeManager aocLifetimeManager)
            {
                _aocLifetimeManager = aocLifetimeManager;
            }

            public void AddPuzzle(Type puzzleType, IPuzzleInfo puzzleInfo)
            {
                _puzzles.Add(puzzleType, puzzleInfo);
                if (_aocLifetimeManager._puzzleModules.ContainsKey(puzzleInfo))
                {
                    throw new ArgumentException(
                        $"{puzzleType.Name}: AOC{puzzleInfo.Year}_{puzzleInfo.Day} has already been added");
                }

                _aocLifetimeManager._puzzleModules.Add(puzzleInfo, _moduleFactory.Create(puzzleType));
            }

            public void Dispose()
            {
                _aocLifetimeManager._puzzleScope = _aocLifetimeManager.container.BeginLifetimeScope(builder =>
                {
                    foreach (var (puzzleType, puzzleIdentification) in _puzzles)
                    {
                        builder.RegisterType(puzzleType).Keyed<IPuzzle>(puzzleIdentification);
                    }
                });

                _aocLifetimeManager.RegisteredPuzzles = _puzzles.Values.ToArray();
            }
        }
    }
}