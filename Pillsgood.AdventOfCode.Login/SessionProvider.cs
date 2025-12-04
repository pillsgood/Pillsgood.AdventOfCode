using System.Diagnostics;
using Pillsgood.AdventOfCode.Common;

namespace Pillsgood.AdventOfCode.Login;

internal sealed class SessionProvider : ISessionProvider
{
    public async ValueTask<string?> GetSessionAsync(CancellationToken cancellationToken = default)
    {
        var hostPath = HostLocator.FindHost();

        var startInfo = new ProcessStartInfo
        {
            FileName = hostPath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = false,
            CreateNoWindow = true,
        };

        using var process = new Process();
        process.StartInfo = startInfo;
        process.EnableRaisingEvents = true;

        try
        {
            if (!process.Start()) throw new Exception("Unable to start login app.");

            await using var stdout = process.StandardOutput.BaseStream;
            using var reader = new StreamReader(stdout);

            var line = await reader.ReadLineAsync(cancellationToken);

            _ = process.WaitForExitAsync(CancellationToken.None);

            if (line?.StartsWith("session=") is not true)
                throw new Exception("Invalid response from login app.");

            return line[8..];
        }
        catch (OperationCanceledException)
        {
            try
            {
                if (!process.HasExited) process.Kill(entireProcessTree: true);
            }
            catch
            {
                /* ignore */
            }

            throw;
        }
    }
}