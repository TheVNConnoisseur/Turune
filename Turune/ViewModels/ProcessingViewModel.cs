using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading;

namespace Turune.ViewModels;

public enum ProcessingState { Running, Succeeded, Failed, Cancelled }
/// <summary>
/// Function that shows the user the current progress for the main process
/// within the called tool.
/// </summary>
/// <param name="navigation">NavigationService model used to go back and forth the menus.</param>
public partial class ProcessingViewModel(NavigationViewModel navigation) : ObservableObject
{
    private readonly NavigationViewModel _navigation = navigation;
    private readonly CancellationTokenSource _cts = new();
    public CancellationToken CancellationToken => _cts.Token;

    [ObservableProperty] public partial string Title { get; set; } = "Processing...";
    [ObservableProperty] public partial double Progress { get; set; }
    [ObservableProperty] public partial bool IsIndeterminate { get; set; } = true;
    [ObservableProperty] public partial string? StatusText { get; set; }
    [ObservableProperty] public partial ProcessingState State { get; set; } = ProcessingState.Running;

    public bool IsRunning => State == ProcessingState.Running;
    public bool IsFinished => State != ProcessingState.Running;

    /// <summary>
    /// Cancel the current process.
    /// </summary>
    /// <remarks>
    /// Only works for all functions that have the CancellationToken as a parameter.
    /// </remarks>
    [RelayCommand(CanExecute = nameof(IsRunning))]
    private void Cancel()
    {
        _cts.Cancel();
        State = ProcessingState.Cancelled;
        StatusText = "Cancelled.";
    }

    /// <summary>
    /// Go back to the menu of the original function.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsFinished))]
    private void Back() => _navigation.GoBack();

    /// <summary>
    /// Go back to the main menu of the application.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsFinished))]
    private void GoHome() => _navigation.GoHome();

    /// <summary>
    /// Function that reevaluates the commands that can be executed
    /// everytime the ProcessingState value changes.
    /// </summary>
    /// <param name="value">Current value of the ProcessingState</param>
    partial void OnStateChanged(ProcessingState value)
    {
        OnPropertyChanged(nameof(IsRunning));
        OnPropertyChanged(nameof(IsFinished));
        CancelCommand.NotifyCanExecuteChanged();
        BackCommand.NotifyCanExecuteChanged();
        GoHomeCommand.NotifyCanExecuteChanged();
    }
}