using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Turune.ViewModels;

//Taken straight from Avalonia Docs: https://docs.avaloniaui.net/docs/how-to/navigation-how-to#navigation-with-back-stack
public partial class NavigationViewModel : ObservableObject
{
    private readonly Stack<ObservableObject> _backStack = new();
    private readonly Stack<ObservableObject> _forwardStack = new();

    [ObservableProperty]
    private ObservableObject? _currentPage;

    public bool CanGoBack => _backStack.Count > 0;
    public bool CanGoForward => _forwardStack.Count > 0;

    public void NavigateTo(ObservableObject page)
    {
        if (CurrentPage is not null)
            _backStack.Push(CurrentPage);

        _forwardStack.Clear();
        CurrentPage = page;

        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(CanGoForward));
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    internal void GoBack()  //The only change done, from private to internal
    {
        if (CurrentPage is not null)
            _forwardStack.Push(CurrentPage);

        CurrentPage = _backStack.Pop();

        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(CanGoForward));
        GoBackCommand.NotifyCanExecuteChanged();
        GoForwardCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanGoForward))]
    private void GoForward()    //Not used, but could be useful in the future
    {
        if (CurrentPage is not null)
            _backStack.Push(CurrentPage);

        CurrentPage = _forwardStack.Pop();

        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(CanGoForward));
        GoBackCommand.NotifyCanExecuteChanged();
        GoForwardCommand.NotifyCanExecuteChanged();
    }
}