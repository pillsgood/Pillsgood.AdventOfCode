namespace Pillsgood.AdventOfCode.Login;

public static class ConfigurationMixins
{
    public static Configuration WithLogin(this Configuration configuration)
    {
        return configuration.WithSessionProvider(static () => new SessionProvider());
    }
}