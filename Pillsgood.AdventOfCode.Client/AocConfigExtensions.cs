using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            return aocConfig.ConfigureServices(collection =>
            {
                collection.TryAddSingleton<IAocClient, AocClient>();
                collection.TryAddSingleton<IAocScraper, AocScraper>();
                collection.TryAddSingleton(provider => (AocClient) provider.GetService<IAocClient>());
                collection.TryAddSingleton(config);
            });
        }
    }
}