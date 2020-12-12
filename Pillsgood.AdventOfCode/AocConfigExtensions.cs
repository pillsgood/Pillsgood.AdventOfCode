using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Pillsgood.AdventOfCode
{
    public static class AocConfigExtensions
    {
        public static AocConfigBuilder LoadAssembly(this AocConfigBuilder builder, params Assembly[] assemblies)
        {
            var current = new List<Assembly>(builder.config.Assemblies);
            current.AddRange(assemblies);
            builder.config.Assemblies = current.Distinct().ToArray();
            return builder;
        }

        public static AocConfigBuilder LoadCallingAssembly(this AocConfigBuilder builder)
        {
            var current = new List<Assembly>(builder.config.Assemblies)
            {
                Assembly.GetCallingAssembly()
            };
            builder.config.Assemblies = current.Distinct().ToArray();
            return builder;
        }

        public static AocConfigBuilder ConfigureServices(this AocConfigBuilder builder, Action<IServiceCollection> configure)
        {
            builder.config.Services += configure;
            return builder;
        }
        
        public static AocConfigBuilder ConfigureServices(this AocConfigBuilder builder, Action<ContainerBuilder> configure)
        {
            builder.config.ContainerConfiguration += configure;
            return builder;
        }
        
        public static AocConfigBuilder PostConfigure(this AocConfigBuilder builder, Action<IServiceCollection> configure)
        {
            builder.config.PostConfiguration += configure;
            return builder;
        }

        public static AocConfigBuilder SetYear(this AocConfigBuilder builder, int year)
        {
            builder.config.Year = year;
            return builder;
        }

        /// <summary>
        /// directory given in format: "Data/Year_{0}/Day_{1}"
        /// directory will be created if it doesnt exist.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AocConfigBuilder SetSerializationPath(this AocConfigBuilder builder, string path = AocConfig.DefaultSerializationPath)
        {
            path = !Path.HasExtension(path) ? path + ".json" : Path.ChangeExtension(path, ".json");
            builder.config.SerializationDirectory = path;
            return builder;
        }
    }
}