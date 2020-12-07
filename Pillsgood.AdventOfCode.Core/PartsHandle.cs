using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Features.Indexed;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PartsHandle
    {
        public delegate PartsHandle Factory(PuzzleMetadata metadata, ILifetimeScope scope);

        public delegate IDisposable ScopeHandle(out Func<string> partHandle, out int part);

        public IEnumerable<ScopeHandle> Values { get; }

        public PartsHandle(PuzzleMetadata metadata, ILifetimeScope scope, IIndex<PuzzleMetadata, IPuzzle> puzzleIndex)
        {
            var puzzleInstance = puzzleIndex[metadata];
            var parts = GetPartMethodInfos(puzzleInstance);
            Values = parts.Select<KeyValuePair<int, MethodInfo>, ScopeHandle>(pair =>
                (out Func<string> handle, out int part) =>
                {
                    var partScope = scope.BeginLifetimeScope();
                    part = pair.Key;
                    handle = InjectParameters(partScope, puzzleInstance, pair.Value);
                    return partScope;
                });
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
    }
}