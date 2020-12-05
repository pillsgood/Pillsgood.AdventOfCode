using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode
{
    public static class AocConfigExtensions
    {
        public static AocConfig LoadAssembly(this AocConfig config, params Assembly[] assemblies)
        {
            var current = new List<Assembly>(config.assemblies);
            current.AddRange(assemblies);
            config.assemblies = current.Distinct().ToArray();
            return config;
        }

        public static AocConfig LoadCallingAssembly(this AocConfig config)
        {
            var current = new List<Assembly>(config.assemblies);
            current.Add(Assembly.GetCallingAssembly());
            config.assemblies = current.Distinct().ToArray();
            return config;
        }

        public static AocConfig ConfigureServices(this AocConfig config, Action<IServiceCollection> configure)
        {
            config.Services += configure;
            return config;
        }

        public static AocConfig SetYear(this AocConfig config, int year)
        {
            config.Year = year;
            return config;    
        }
    }
}