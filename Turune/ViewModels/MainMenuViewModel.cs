using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using Turune.Models;
using Turune.Services;
using System;
using Turune.Models.Formats.Containers;
using System.IO;

namespace Turune.ViewModels;

public partial class MainMenuViewModel(string title, IReadOnlyList<MenuOption> options, IStorageService storageService) : ObservableObject
{
    private readonly IStorageService _storageService = storageService;
    public string Title { get; } = title;
    public IReadOnlyList<MenuOption> Options { get; } = options;

    //Each level of the menu has its own selected option and sub-options
    [ObservableProperty]
    public partial MenuOption? SelectedOption1 { get; set; }

    [ObservableProperty]
    public partial IReadOnlyList<MenuOption>? SubOptions1 { get; set; }

    [ObservableProperty]
    public partial MenuOption? SelectedOption2 { get; set; }

    [ObservableProperty]
    public partial IReadOnlyList<MenuOption>? SubOptions2 { get; set; }

    [ObservableProperty]
    public partial MenuOption? SelectedOption3 { get; set; }

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
    
    
    /// <summary>
    /// Executes the action of the currently selected option, prioritizing the deepest selection.
    /// </summary>
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
    }

    // private ObservableCollection<IniFile> _loadedInis = new();

    [ObservableProperty]
    public partial byte[][] ExportedODN { get; set; } = Array.Empty<byte[]>();
    private readonly IFileLoader _fileLoader = new FileLoader();

    // Sample code to demonstrate how to use the StorageService
    [RelayCommand]
    private async Task Test()
    {
        var files = await _storageService.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open File",
            AllowMultiple = true
        });

        if (files.Count < 1) return;

        foreach (var file in files)
        {
            var path = file.Path.LocalPath;
            var bytes = await _fileLoader.ReadBytesAsync(path);

            var odn = new ODN();
            odn.ParsePacked(bytes);
            var saveFile = await _storageService.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Save Exported Files",
                AllowMultiple = false,
            });
            if (saveFile is not null)
            {
                foreach (var entry in odn.OdnScheme.Entries)
                {
                    var savePath = Path.Combine(saveFile[0].Path.LocalPath, entry.Name);
                    await _fileLoader.WriteBytesAsync(savePath, entry.Data);
                }
            }
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