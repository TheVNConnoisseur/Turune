using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Turune.Models.Formats.Initialization;
using Turune.Services;

namespace Turune.ViewModels.Tools.Initialization;
public partial class DATExportViewModel(
    IStorageService storageService,
    IFileIO fileIO,
    NavigationViewModel navigation) : ObservableObject
{
    private readonly IStorageService _storageService = storageService;
    private readonly IFileIO _fileIO = fileIO;
    private readonly NavigationViewModel _navigation = navigation;

    //DAT elements
    private DAT dat = new();
    public byte[] fileData = [];

    //Properties screen
    [ObservableProperty] public partial string? FileIOStatus { get; set; } = "Please select a file.";
    [ObservableProperty] public partial bool IsReady { get; set; } = false;
    [ObservableProperty] public partial bool IsBusy { get; set; } = false;

    [RelayCommand] private void GoBack() => _navigation.GoBack();

    [RelayCommand]
    private async Task SelectInputFile()
    {
        try
        {
            var inputFile = await _storageService.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select file to open",
                FileTypeFilter = [Models.FileType.Initialization.DAT],
                SuggestedFileType = Models.FileType.Initialization.DAT
            });
            if (inputFile.Count == 0)
            {
                //Only set the sentence of no file selected in case the user already selected previously an accepted file.
                if (IsReady == false)
                    FileIOStatus = "No file was selected.";
                return;
            }
            
            IsBusy = true;
            IsReady = false;
            
            dat = new DAT();
            fileData = await _fileIO.ReadBytesAsync(inputFile[0].Path.LocalPath);
            dat.ParsePacked(fileData);

            FileIOStatus = inputFile[0].Name + " parsed correctly.";
            IsReady = true;
        }
        catch (Exception ex)
        {
            FileIOStatus = ex.Message;
            IsReady = false;
            IsBusy = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Extract()
    {
        var saveFile = await _storageService.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save file",
            DefaultExtension = "ini",
            FileTypeChoices = [Models.FileType.Initialization.INI]
        });
        if (saveFile is null) return;

        //Pass the entire process onto an external VM
        var processingVM = new ProcessingViewModel(_navigation)
        {
            Title = "Exporting file...",
            IsIndeterminate = true
        };

        _navigation.NavigateTo(processingVM);

        try
        {
            processingVM.StatusText = "Building file structure...";
            processingVM.CancellationToken.ThrowIfCancellationRequested();
            string[] exportedFile = dat.Export();

            processingVM.StatusText = "Writing to disk...";
            await _fileIO.WriteLinesAsync(saveFile.Path.LocalPath, exportedFile, processingVM.CancellationToken);
            
            processingVM.Progress = 100;
            processingVM.IsIndeterminate = false;
            processingVM.StatusText = "Done.";
            processingVM.State = ProcessingState.Succeeded;
        }
        catch (OperationCanceledException)
        {
            //Cancelling process handled by ProcessingViewModel.Cancel()
        }
        catch (Exception ex)
        {
            processingVM.StatusText = ex.Message;
            processingVM.State = ProcessingState.Failed;
        }
    }
}