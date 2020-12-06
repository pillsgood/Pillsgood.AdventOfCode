using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Autofac.Builder;
using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PuzzleMetadataConfiguration
    {
        public delegate PuzzleMetadataConfiguration Factory(Assembly assembly);

        private readonly int? _assemblyYear;

        public PuzzleMetadataConfiguration(Assembly assembly, IAocConfig config)
        {
            _assemblyYear = config.Year.HasValue && config.Year.Value > 2000
                ? config.Year
                : assembly.GetCustomAttribute<AocYearAttribute>()?.Year
                  ?? GetAssemblyYear(assembly);
        }

        public Action<MetadataConfiguration<PuzzleMetadata>> From(Type type)
        {
            var metadata = type.GetCustomAttribute<PuzzleAttribute>()?.metadata ?? new PuzzleMetadata();
            metadata.Type = type;
            metadata.year ??= _assemblyYear ?? GetYear(type);
            metadata.day ??= GetDay(type);
            return configuration => configuration
                .For(puzzleMetadata => puzzleMetadata.Type, type)
                .For(puzzleMetadata => puzzleMetadata.Year, metadata.Year)
                .For(puzzleMetadata => puzzleMetadata.Day, metadata.Day);
        }

        private static int? GetDay(MemberInfo type)
        {
            if (type.Name.Contains("day", StringComparison.OrdinalIgnoreCase) && int.TryParse(
                new string(type.Name.Where(char.IsDigit).ToArray()), NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo, out var day))
            {
                return day;
            }

            return null;
        }

        private static int? GetYear(Type type)
        {
            if (!string.IsNullOrEmpty(type.Namespace) && int.TryParse(
                new string(type.Namespace.Where(char.IsDigit).ToArray()), NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo, out var year))
            {
                return year;
            }

            return null;
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
    }
}