using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Module = Autofac.Module;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PuzzleModule<T> : Module where T : IPuzzle
    {
        protected override void Load(ContainerBuilder builder)
        {
            var services = new ServiceCollection();
            var methodInfo = typeof(T).GetMethod(nameof(IPuzzle.ConfigureServices)) ??
                             throw new ArgumentNullException();
            var registerAction = Delegate.CreateDelegate(typeof(Action<IServiceCollection>), null, methodInfo);
            registerAction.DynamicInvoke(services);
            builder.Populate(services);
        }
    }
}