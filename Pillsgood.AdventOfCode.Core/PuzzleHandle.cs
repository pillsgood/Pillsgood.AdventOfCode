using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Features.Indexed;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PuzzleHandle : IDisposable
    {
        public delegate PuzzleHandle Factory(PuzzleMetadata metadata);

        private readonly ILifetimeScope _scope;

        public delegate IDisposable PartScopeHandle(out Func<string> partHandle);

        public IEnumerable<KeyValuePair<int, PartScopeHandle>> Values { get; }

        public PuzzleHandle(PuzzleMetadata metadata, ILifetimeScope scope, IIndex<PuzzleMetadata, IPuzzle> puzzleIndex)
        {
            _scope = scope.BeginLifetimeScope(builder =>
            {
                builder.RegisterModule(PuzzleModuleFactory.Create(metadata));
                builder.RegisterType<PuzzleInput>().As<IPuzzleInput>()
                    .WithParameter(new TypedParameter(typeof(PuzzleMetadata), metadata)).SingleInstance();
            });

            var puzzleInstance = puzzleIndex[metadata];
            var parts = GetPartMethodInfos(puzzleInstance);
            Values = parts.Select(pair => new KeyValuePair<int, PartScopeHandle>(pair.Key, (out Func<string> handle) =>
            {
                var partScope = _scope.BeginLifetimeScope();
                handle = InjectParameters(partScope, puzzleInstance, pair.Value);
                return partScope;
            }));
        }

        private static Func<string> InjectParameters(IComponentContext scope, IPuzzle puzzle, MethodInfo methodInfo)
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
            return () => (string) methodInfo.Invoke(puzzle, parameters.ToArray());
        }

        private static SortedDictionary<int, MethodInfo> GetPartMethodInfos(IPuzzle puzzle)
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
            _scope?.Dispose();
        }
    }
}