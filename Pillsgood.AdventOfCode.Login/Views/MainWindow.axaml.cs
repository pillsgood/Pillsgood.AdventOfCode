using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Pillsgood.AdventOfCode.Login.ViewModels;

namespace Pillsgood.AdventOfCode.Login.Views;

internal class MainWindow : ReactiveWindow<MainViewModel>
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
    }
}