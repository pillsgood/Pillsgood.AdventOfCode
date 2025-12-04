using Avalonia;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Pillsgood.AdventOfCode.Common;
using Pillsgood.AdventOfCode.Login.ViewModels;

namespace Pillsgood.AdventOfCode.Login;

internal class SessionProvider : ISessionProvider
{
    [STAThread]
    private static void Execute() => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(Array.Empty<string>());

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();

    public async ValueTask<string?> GetSessionAsync(CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<string?>();

        CookieVisitor.SessionCookieVisited
            .Where(static x => !string.IsNullOrEmpty(x.Value))
            .Subscribe(cookie =>
            {
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        lifetime.Shutdown();
                        tcs.TrySetResult(cookie.Value);
                    });
                }
            });

        Execute();

        return await tcs.Task;
    }
}