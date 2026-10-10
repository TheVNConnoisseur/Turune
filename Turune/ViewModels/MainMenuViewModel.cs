using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using Turune.Models;
using Turune.Services;

namespace Turune.ViewModels;

public partial class MainMenuViewModel(
    string title,
    IReadOnlyList<MenuOption> options,
    NavigationViewModel navigation,
    IToolViewModelFactory toolFactory) : ObservableObject
{
    private readonly NavigationViewModel _navigation = navigation;
    private readonly IToolViewModelFactory _toolFactory = toolFactory;

    public string Title { get; } = title;
    public IReadOnlyList<MenuOption> Options { get; } = options;

    //Each level of the menu has its own selected option and sub-options
    [ObservableProperty] public partial MenuOption? SelectedOption1 { get; set; }

    [ObservableProperty] public partial IReadOnlyList<MenuOption>? SubOptions1 { get; set; }

    [ObservableProperty] public partial MenuOption? SelectedOption2 { get; set; }

    [ObservableProperty] public partial IReadOnlyList<MenuOption>? SubOptions2 { get; set; }

    [ObservableProperty] public partial MenuOption? SelectedOption3 { get; set; }

    //Returns the description of the currently selected option, prioritizing the deepest selection
    public string? CurrentDescription =>
        SelectedOption3?.Description ??
        SelectedOption2?.Description ??
        SelectedOption1?.Description;

    //To know when the user can go to the next step
    private bool HasSelection =>
        !string.IsNullOrEmpty(SelectedOption3?.ToolKey) ||
        !string.IsNullOrEmpty(SelectedOption2?.ToolKey) ||
        !string.IsNullOrEmpty(SelectedOption1?.ToolKey);
    
    
    /// <summary>
    /// Executes the action of the currently selected option, prioritizing the deepest selection.
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void Next()
    {
        var target = SelectedOption3 ?? SelectedOption2 ?? SelectedOption1;
        if (target is null) return;

        if (!string.IsNullOrEmpty(target.ToolKey))
        {
            var vm = _toolFactory.Create(target.ToolKey);
            _navigation.NavigateTo(vm);
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