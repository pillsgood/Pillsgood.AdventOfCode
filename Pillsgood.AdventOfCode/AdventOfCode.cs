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

            return lifetimeManager.container.Resolve<AdventOfCode>().Load();
        }


        private IPuzzleRunner Load()
        {
            return _lifetimeManager.CreateRunner();
        }
    }
}