using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Turune.Models.Formats.Images;
using Turune.Services;

namespace Turune.ViewModels.Tools.Images;
public partial class RAWExportViewModel(
    IStorageService storageService,
    IFileIO fileIO,
    NavigationViewModel navigation) : ObservableObject
{
    private readonly IStorageService _storageService = storageService;
    private readonly IFileIO _fileIO = fileIO;
    private readonly NavigationViewModel _navigation = navigation;

    // //RAW properties
    // [ObservableProperty] public partial string IsEncrypted { get; set; } = "-";
    // [ObservableProperty] public partial string EntriesOffsetsAreRelativeToRawData { get; set; } = "-";
    // [ObservableProperty] public partial string HasDelimiterEntry { get; set; } = "-";

    //RAW elements
    private RAW raw = new();
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
            var compressedFile = await _storageService.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select a compressed file to open",
                AllowMultiple = true
            });
            if (compressedFile.Count == 0)
            {
                //Only set the sentence of no file selected in case the user already selected previously an accepted file.
                if (IsReady == false)
                    FileIOStatus = "No file was selected.";
                return;
            }
            
            IsBusy = true;
            IsReady = false;
            
            raw = new RAW();
            fileData = await _fileIO.ReadBytesAsync(compressedFile[0].Path.LocalPath);
            raw.ParsePacked(fileData);

            FileIOStatus = compressedFile[0].Name + " parsed correctly.";
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

    // [RelayCommand]
    // private async Task Extract()
    // {
    //     var saveFolder = await _storageService.OpenFolderPickerAsync(new FolderPickerOpenOptions
    //     {
    //         Title = "Select the folder to export the files to"
    //     });
    //     if (saveFolder.Count == 0) return;

    //     //Pass the entire process onto an external VM
    //     var processingVM = new ProcessingViewModel(_navigation)
    //     {
    //         Title = "Exporting files from ODN...",
    //         IsIndeterminate = false
    //     };

    //     _navigation.NavigateTo(processingVM);

    //     try
    //     {
    //         processingVM.CancellationToken.ThrowIfCancellationRequested();
            
    //         //Write all of the files inside the selected ODN to the selected path
    //         for (int i = 0; i < odn.OdnScheme.Entries.Count; i++)
    //         {
    //             processingVM.StatusText = "Writing file " + i + " of " + odn.OdnScheme.Entries.Count + " to disk...";
    //             string saveFilePath = Path.Combine(saveFolder[0].Path.LocalPath, odn.OdnScheme.Entries[i].Name);
    //             await _fileIO.WriteBytesAsync(saveFilePath, odn.OdnScheme.Entries[i].Data, processingVM.CancellationToken);

    //             processingVM.Progress = (i + 1) / (double)odn.OdnScheme.Entries.Count * 100;
    //         }
            
    //         processingVM.StatusText = "Done.";
    //         processingVM.State = ProcessingState.Succeeded;
    //     }
    //     catch (OperationCanceledException)
    //     {
    //         //Cancelling process handled by ProcessingViewModel.Cancel()
    //     }
    //     catch (Exception ex)
    //     {
    //         processingVM.StatusText = ex.Message;
    //         processingVM.State = ProcessingState.Failed;
    //     }
    // }

}