using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Threading;
using ReactiveUI;
using Xilium.CefGlue;

namespace Pillsgood.AdventOfCode.Login.ViewModels;

internal class MainViewModel : ReactiveObject, IActivatableViewModel
{
    private const string AuthAddress = "https://adventofcode.com/auth/login";

    private string? _address = AuthAddress;
    private bool _webViewVisible = true;

    public MainViewModel()
    {
        this.WhenActivated(OnActivated);
    }

    private void OnActivated(CompositeDisposable dispose)
    {
        Address = AuthAddress;

        this.WhenAnyValue(vm => vm.Address)
            .DistinctUntilChanged()
            .Subscribe(OnNextAddress)
            .DisposeWith(dispose);

        CookieVisitor.SessionCookieVisited
            .Subscribe(_ => WebViewVisible = false)
            .DisposeWith(dispose);
    }

    private void OnNextAddress(string? address)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var visitor = new CookieVisitor();
            CefCookieManager.GetGlobal(null).VisitAllCookies(visitor);
        });
    }

    public string? Address
    {
        get => _address;
        set => this.RaiseAndSetIfChanged(ref _address, value);
    }

    public bool WebViewVisible
    {
        get => _webViewVisible;
        set => this.RaiseAndSetIfChanged(ref _webViewVisible, value);
    }

    public ViewModelActivator Activator { get; } = new();
}