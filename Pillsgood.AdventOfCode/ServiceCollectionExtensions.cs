using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Common;
using Pillsgood.AdventOfCode.Common.InputConverters;

namespace Pillsgood.AdventOfCode;

internal static class ServiceCollectionExtensions
{
    extension(ServiceCollection services)
    {
        public void AddInputServices()
        {
            services.AddSingleton<IPuzzleInputService, InputService>();
        }

        public void AddAssertionServices()
        {
            services.AddSingleton<IAnswerService, AnswerService>();
            services.AddSingleton<IAnswerAssertion, Assertion>();
        }

        public void AddInputConverters()
        {
            services.AddTransient<IPuzzleInputReader, PuzzleInputReader>();

            services.AddTransient(typeof(IConverterService<>), typeof(ConverterService<>));
            services.AddTransient(typeof(ICollectionConverterService<,>), typeof(CollectionConverterService<,>));

            services.AddTransient<IInputConverter<string, string>, IdentityConverter>();
            services.AddTransient(typeof(IInputConverter<,>), typeof(ParsableInputConverter<,>));
            services.AddTransient(typeof(IInputConverter<,>), typeof(NumberInputConverter<,>));
            services.AddTransient(typeof(ElementConverter<,>));
            services.AddTransient(typeof(ICollectionConverter<,>), typeof(ReflectionCollectionConverter<,>));
            services.AddTransient(typeof(ICollectionConverter<,>), typeof(ArrayCollectionConverter<,>));
        }
    }
}