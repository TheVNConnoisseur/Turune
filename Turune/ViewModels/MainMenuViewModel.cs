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

    //Each level of the menu has its own selected option and sub-options
    [ObservableProperty] private MenuOption? _selectedOption1;  //Selected option at the first column
    [ObservableProperty] private IReadOnlyList<MenuOption>? _subOptions1; //Sub-options for the first column
    [ObservableProperty] private MenuOption? _selectedOption2;  //Selected option at the second column
    [ObservableProperty] private IReadOnlyList<MenuOption>? _subOptions2; //Sub-options for the second column
    [ObservableProperty] private MenuOption? _selectedOption3;  //Selected option at the third column

    //Returns the description of the currently selected option, prioritizing the deepest selection
    public string? CurrentDescription =>
        SelectedOption3?.Description ??
        SelectedOption2?.Description ??
        SelectedOption1?.Description;

    //To know when the user can go to the next step, we check if any of the selected options has an Execute action
    private bool HasSelection => 
        SelectedOption3?.Execute is not null ||
        SelectedOption2?.Execute is not null ||
        SelectedOption1?.Execute is not null;

    [RelayCommand(CanExecute = nameof(HasSelection))]

    /// <summary>
    /// Executes the action of the currently selected option, prioritizing the deepest selection,
    /// </summary>
    private void Next()
    {
        var target = SelectedOption3 ?? SelectedOption2 ?? SelectedOption1;
        if (target is null) return;

        if (target.Execute is not null)
        {
            target.Execute.Invoke();
            return;
        }
    }

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