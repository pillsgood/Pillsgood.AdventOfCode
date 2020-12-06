namespace Pillsgood.AdventOfCode
{
    public class AocConfigBuilder
    {
        internal readonly AocConfig config;

        internal AocConfigBuilder()
        {
            config = new AocConfig();
        }

        internal AocConfigBuilder(AocConfig config)
        {
            this.config = config;
        }
    }
}