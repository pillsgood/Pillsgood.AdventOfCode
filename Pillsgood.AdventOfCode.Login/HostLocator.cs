using System.Reflection;
using System.Runtime.InteropServices;

namespace Pillsgood.AdventOfCode.Login;

internal static class HostLocator
{
    public static string FindHost()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || !Environment.Is64BitProcess)
        {
            throw new PlatformNotSupportedException();
        }

        var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                          ?? AppContext.BaseDirectory;

        const string rid = "win-x64";
        const string fileName = "Pillsgood.AdventOfCode.Login.App.exe";

        var devPath = Path.Combine(assemblyDir, fileName);
        if (File.Exists(devPath))
            return devPath;

        var nugetPath = Path.Combine(assemblyDir, "runtimes", rid, "native", fileName);
        if (File.Exists(nugetPath))
            return nugetPath;

        throw new FileNotFoundException($"Login app was not found for RID '{rid}'. Looked in: '{devPath}' and '{nugetPath}'.");
    }
}