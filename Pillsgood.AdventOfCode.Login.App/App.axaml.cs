using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace Pillsgood.AdventOfCode.Login.App;

public partial class App : Application
{
    private MainWindow? _window;
    private IClassicDesktopStyleApplicationLifetime? _lifetime;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _lifetime = desktop;
            _window = new MainWindow();
            _window.WebView.NavigationCompleted += WebViewOnNavigationCompleted;
            desktop.MainWindow = _window;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async void WebViewOnNavigationCompleted(object? sender, WebViewNavigationCompletedEventArgs _)
    {
        try
        {
            var cookieManager = _window?.WebView.TryGetCookieManager();
            if (cookieManager == null) return;

            var cookies = await cookieManager.GetCookiesAsync();
            var session = cookies.FirstOrDefault(cookie => cookie is { Domain: ".adventofcode.com", Name: "session" })?.Value;
            if (string.IsNullOrEmpty(session)) return;

            await Console.Out.WriteLineAsync($"session={session}");
            await Console.Out.FlushAsync();

            Dispatcher.UIThread.Post(() =>
            {
                _window?.WebView.IsVisible = false;
                _window?.CompletionMessage.IsVisible = true;
                _lifetime?.Shutdown();
            });
        }
        catch (Exception)
        {
            _lifetime?.Shutdown(1);
        }
    }
}