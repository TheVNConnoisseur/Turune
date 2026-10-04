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

    [ObservableProperty] private MenuOption? _selectedOption1;
    [ObservableProperty] private IReadOnlyList<MenuOption>? _subOptions1;
    [ObservableProperty] private MenuOption? _selectedOption2;
    [ObservableProperty] private IReadOnlyList<MenuOption>? _subOptions2;
    [ObservableProperty] private MenuOption? _selectedOption3;

    public string? CurrentDescription =>
        SelectedOption3?.Description ??
        SelectedOption2?.Description ??
        SelectedOption1?.Description;

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void Next()
    {
        var target = SelectedOption3 ?? SelectedOption2 ?? SelectedOption1;
        if (target is null) return;

        if (target.Execute is not null)
        {
            target.Execute.Invoke();
            return;
        }

        if (target.HasChildren)
            _nav.NavigateTo(new MainMenuViewModel(_nav, target.Name, target.Children));
    }

    private bool HasSelection => SelectedOption1 is not null;

    partial void OnSelectedOption1Changed(MenuOption? value)
    {
        SubOptions1 = value?.HasChildren == true ? value.Children : null;
        SelectedOption2 = null;
        SelectedOption3 = null;
        OnPropertyChanged(nameof(CurrentDescription));
        NextCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedOption2Changed(MenuOption? value)
    {
        SubOptions2 = value?.HasChildren == true ? value.Children : null;
        SelectedOption3 = null;
        OnPropertyChanged(nameof(CurrentDescription));
        NextCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedOption3Changed(MenuOption? value)
    {
        OnPropertyChanged(nameof(CurrentDescription));
        NextCommand.NotifyCanExecuteChanged();
    }
}