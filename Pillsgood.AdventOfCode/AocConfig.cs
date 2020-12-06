using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode
{
    internal class AocConfig : IAocConfig
    {
        private const string DefaultSerializationPath = "Data/Year_{0}/Day_{1}.json";

        internal AocConfig()
        {
        }

        public Action<IServiceCollection> Services { get; internal set; } = delegate { };
        public int? Year { get; internal set; } = null;
        public Assembly[] Assemblies { get; internal set; } = new Assembly[0];
        public LogLevel LoggingLevel { get; internal set; } = LogLevel.None;
        public string SerializationDirectory { get; internal set; } = DefaultSerializationPath;
    }
}