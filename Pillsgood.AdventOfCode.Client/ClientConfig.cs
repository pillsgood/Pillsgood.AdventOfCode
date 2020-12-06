using System;

namespace Pillsgood.AdventOfCode.Client
{
    public class AocClientConfig
    {
        internal string SessionId { get; set; } = Environment.GetEnvironmentVariable("AOC_SESSION");

        /// <summary>
        /// Not Safe! instead try setting AOC_SESSION in environment variables
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns>AocClientConfig</returns>
        public AocClientConfig SetSessionId(string sessionId)
        {
            SessionId = sessionId.Replace("session=", "");
            return this;
        }
    }
}