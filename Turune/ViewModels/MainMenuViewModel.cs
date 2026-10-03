using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using Turune.Models;

namespace Turune.ViewModels;

public partial class MainMenuViewModel : ObservableObject
{
    private readonly NavigationViewModel _nav;

    public string Title { get; }
    public IReadOnlyList<MenuOption> Options { get; }
    public bool IsRootMenu { get; }
    public bool CanGoBack => !IsRootMenu;

    [ObservableProperty]
    private MenuOption? _selectedOption;

    public MainMenuViewModel(NavigationViewModel nav, string title, IReadOnlyList<MenuOption> options, bool isRootMenu = false)
    {
        _nav = nav;
        Title = title;
        Options = options;
        IsRootMenu = isRootMenu;

        if (options.Count > 0)
            SelectedOption = options[0];
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private void GoBack() => _nav.GoBack();

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void Next()
    {
        if (SelectedOption is null) return;

        if (SelectedOption.Execute is not null)
        {
            SelectedOption.Execute.Invoke();
            return;
        }

        if (SelectedOption.HasChildren)
        {
            _nav.NavigateTo(new MainMenuViewModel(
                _nav,
                SelectedOption.Key,
                SelectedOption.Children,
                isRootMenu: false));
        }
    }

    private bool HasSelection => SelectedOption is not null;

    partial void OnSelectedOptionChanged(MenuOption? value)
    {
        NextCommand.NotifyCanExecuteChanged();
    }
}