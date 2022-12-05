using Avalonia;
using System;
using System.Reactive.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Pillsgood.AdventOfCode.Common;
using Pillsgood.AdventOfCode.Login.ViewModels;

namespace Pillsgood.AdventOfCode.Login;

internal class SessionProvider : SessionProviderBase
{
    [STAThread]
    private static void Execute() => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(Array.Empty<string>());

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();

    protected override string? GetSession()
    {
        var session = string.Empty;

        CookieVisitor.SessionCookieVisited
            .Where(static x => !string.IsNullOrEmpty(x.Value))
            .Subscribe(cookie =>
            {
                session = cookie.Value;
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                {
                    Dispatcher.UIThread.Post(() => lifetime.Shutdown());
                }
            });

        Execute();

        return session;
    }
}