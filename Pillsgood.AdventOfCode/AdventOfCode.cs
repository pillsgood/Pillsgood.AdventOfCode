using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace Pillsgood.AdventOfCode
{
    public class AdventOfCode
    {
        private readonly AocConfig _aocConfig;
        private readonly AocLifetimeManager _lifetimeManager;

        private bool OneYear => _aocConfig.Year.HasValue && _aocConfig.Year.Value > 2000;

        private AdventOfCode(AocConfig aocConfig, AocLifetimeManager lifetimeManager, IAocConsole aocConsole = null)
        {
            _aocConfig = aocConfig;
            _lifetimeManager = lifetimeManager;
            aocConsole?.StartUpMessage();
        }

        public static IPuzzleRunner Build(Action<AocConfig> configure)
        {
            var config = new AocConfig();
            configure.Invoke(config);
            var services = new ServiceCollection();
            config.Services.Invoke(services);

            var lifetimeManager = AocLifetimeManager.Build(builder =>
            {
                builder.RegisterInstance(config).SingleInstance();
                builder.RegisterType<AdventOfCode>().SingleInstance().FindConstructorsWith(new AllConstructorFinder());
                builder.Populate(services);
            });

            return lifetimeManager.container.Resolve<AdventOfCode>().Load(config.assemblies);
        }


        private IPuzzleRunner Load(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                RegisterPuzzles(assembly);
            }

            return _lifetimeManager.CreateRunner();
        }

        private void RegisterPuzzles(Assembly assembly)
        {
            var assemblyYear = OneYear
                ? _aocConfig.Year
                : assembly.GetCustomAttribute<AocYearAttribute>()?.Year
                  ?? GetAssemblyYear(assembly);

            using var scope = _lifetimeManager.BeginPuzzleRegistration(out var registerPuzzleType);
            foreach (var type in assembly.GetTypes().Where(type =>
                type.GetInterfaces().Any(interfaceType => interfaceType == typeof(IPuzzle))))
            {
                var puzzleAttribute = type.GetCustomAttribute<PuzzleAttribute>() ?? new PuzzleAttribute();
                try
                {
                    puzzleAttribute.Year ??= assemblyYear ?? GetClassYear(type);
                }
                catch (ArgumentNullException)
                {
                    if (!OneYear)
                    {
                        throw;
                    }

                    puzzleAttribute.Year = _aocConfig.Year;
                }

                puzzleAttribute.Day ??= GetDay(type);
                registerPuzzleType.Invoke(type, puzzleAttribute);
            }
        }

        private static int? GetAssemblyYear(Assembly assembly)
        {
            var charArray = assembly.GetName().Name?.Where(char.IsDigit).ToArray();
            if (charArray == null || charArray.Length == 0)
            {
                return null;
            }

            var digits = new string(charArray);
            var startIdx = digits.IndexOf("20", StringComparison.OrdinalIgnoreCase);
            if (startIdx == -1)
            {
                return null;
            }

            var year = int.Parse(digits[startIdx..]);
            if (year < 3000 && year >= 2015)
            {
                return year;
            }

            return null;
        }

        private static int GetClassYear(Type type)
        {
            if (type.Name.Contains("day", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(type.Namespace) &&
                int.TryParse(new string(type.Namespace.Where(char.IsDigit).ToArray()), NumberStyles.Integer,
                    NumberFormatInfo.InvariantInfo, out var year))
            {
                return year;
            }

            throw new ArgumentNullException(
                $"Failed to get year of {type.Name}. define year in AocConfig or using AocYear assembly attribute; year can also be inferred from namespace/assembly name");
        }

        private static int GetDay(MemberInfo type)
        {
            if (type.Name.Contains("day", StringComparison.OrdinalIgnoreCase) && int.TryParse(
                new string(type.Name.Where(char.IsDigit).ToArray()), NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo, out var day))
            {
                return day;
            }

            throw new ArgumentNullException(
                $"Failed to get day of {type.Name}. use PuzzleAttribute or rename class to Day** for inference");
        }
    }
}