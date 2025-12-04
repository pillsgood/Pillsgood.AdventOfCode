using System;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using WebViewControl;
using Xilium.CefGlue;

namespace Pillsgood.AdventOfCode.Login.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = new MainWindow();
            desktop.MainWindow = window;

            var subject = new Subject<string>();

            window.WebView.GetPropertyChangedObservable(WebView.AddressProperty)
                .Subscribe(_ =>
                    Dispatcher.UIThread.Post(() =>
                    {
                        try
                        {
                            var cookieManager = CefCookieManager.GetGlobal(null);
                            cookieManager.VisitAllCookies(new CookieVisitor(subject));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }));

            subject.Subscribe(session =>
            {
                Console.Out.WriteLine($"session={session}");
                Console.Out.Flush();

                Dispatcher.UIThread.Post(() =>
                {
                    window.WebView.IsVisible = false;
                    window.CompletionMessage.IsVisible = true;
                    desktop.Shutdown();
                });
            });
        }

        base.OnFrameworkInitializationCompleted();
    }
}