using Microsoft.Extensions.DependencyInjection;

namespace Pillsgood.AdventOfCode.Abstractions
{
    public interface IPuzzle
    {
        void ConfigureServices(IServiceCollection services);
    }
}