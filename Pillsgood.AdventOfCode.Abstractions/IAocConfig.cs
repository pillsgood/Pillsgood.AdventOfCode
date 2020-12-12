using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Pillsgood.AdventOfCode.Abstractions
{
    internal interface IAocConfig
    {
        int? Year { get; }
        Action<IServiceCollection> Services { get; }
        Assembly[] Assemblies { get; }
        string SerializationDirectory { get; }
        Action<IServiceCollection> PostConfiguration { get; }
    }
}