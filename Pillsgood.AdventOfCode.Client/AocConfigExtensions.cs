using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Client
{
    public static class AocConfigExtensions
    {
        public static AocConfigBuilder AddClient(this AocConfigBuilder aocConfig)
        {
            aocConfig.ConfigureServices(collection => collection.TryAddSingleton<IAocClient, AocClient>());
            return aocConfig;
        }

        public static AocConfigBuilder AddClient(this AocConfigBuilder aocConfig, Action<AocClientConfig> configure)
        {
            var config = new AocClientConfig();
            configure.Invoke(config);
            return aocConfig.AddClient()
                .ConfigureServices(collection => collection
                    .TryAddSingleton(config));
        }
    }
}