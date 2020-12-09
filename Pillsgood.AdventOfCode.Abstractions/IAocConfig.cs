using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pillsgood.AdventOfCode.Abstractions
{
    internal interface IAocConfig
    {
        int? Year { get; }
        Action<IServiceCollection> Services { get; }
        LogLevel LoggingLevel { get; }
        Assembly[] Assemblies { get; }
        string SerializationDirectory { get; }
        Action<IServiceCollection> PostConfiguration { get; }
    }
}