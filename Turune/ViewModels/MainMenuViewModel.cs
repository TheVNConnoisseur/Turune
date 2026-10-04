using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using Turune.Models;

namespace Turune.ViewModels;

public partial class MainMenuViewModel(NavigationViewModel nav, string title, IReadOnlyList<MenuOption> options) : ObservableObject
{
    private readonly NavigationViewModel _nav = nav;

    public string Title { get; } = title;
    public IReadOnlyList<MenuOption> Options { get; } = options;

    [ObservableProperty]
    private MenuOption? _selectedOption;

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
    }

    private bool HasSelection => SelectedOption is not null;

    partial void OnSelectedOptionChanged(MenuOption? value)
    {
        NextCommand.NotifyCanExecuteChanged();
    }
}