using System;
using Autofac;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Client
{
    public static class AocConfigExtensions
    {
        public static AocConfigBuilder AddClient(this AocConfigBuilder aocConfig)
        {
            aocConfig.AddClient(null);
            return aocConfig;
        }

        public static AocConfigBuilder AddClient(this AocConfigBuilder aocConfig, Action<AocClientConfig> configure)
        {
            var config = new AocClientConfig();
            configure?.Invoke(config);
            return aocConfig.ConfigureServices(builder =>
            {
                builder.RegisterType<AocClient>().AsSelf().As<IAocClient>().SingleInstance();
                builder.RegisterType<AocWebSession>().As<IAocWebSession>().InstancePerLifetimeScope();
                builder.RegisterInstance(config).AsSelf().SingleInstance();
            });
        }
    }
}