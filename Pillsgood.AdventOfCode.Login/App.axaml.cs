using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Pillsgood.AdventOfCode.Login.ViewModels;
using Pillsgood.AdventOfCode.Login.Views;

namespace Pillsgood.AdventOfCode.Login;

internal class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow()
            {
                ViewModel = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}