namespace Pillsgood.AdventOfCode.Login;

public static class ConfigurationExtensions
{
    extension(Configuration configuration)
    {
        public Configuration WithLogin()
        {
            return configuration.WithSessionProvider(static () => new SessionProvider());
        }
    }
}