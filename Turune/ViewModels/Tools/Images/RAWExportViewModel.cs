using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
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

    //RAW elements
    private RAW raw = new();
    public ObservableCollection<FileItem> FileItems { get; } = [];
    public class FileItem
    {
        public required string Name { get; set; } = string.Empty;
        public required string Path { get; set; } = string.Empty;
    }

    //Properties screen
    [ObservableProperty] public partial Bitmap? PreviewImage { get; set; }
    [ObservableProperty] public partial string? FileIOStatus { get; set; } = "Please select a file.";
    [ObservableProperty] public partial bool IsReady { get; set; } = false;
    [ObservableProperty] public partial bool IsBusy { get; set; } = false;
    [ObservableProperty] public partial double Progress { get; set; } = 0;

    [RelayCommand] private void GoBack() => _navigation.GoBack();

    [RelayCommand]
    private async Task SelectInputFile()
    {
        try
        {
            var compressedFiles = await _storageService.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select a compressed image to open",
                AllowMultiple = true
            });
            if (compressedFiles.Count == 0)
            {
                //Only set the sentence of no file selected in case the user already selected previously accepted files.
                if (IsReady == false)
                    FileIOStatus = "No file was selected.";
                return;
            }
            
            Progress = 0;
            IsBusy = true;
            IsReady = false;
            PreviewImage = null;

            FileItems.Clear();
            for (int i = 0; i < compressedFiles.Count; i++)
            {
                try
                {
                    byte[] fileData = await _fileIO.ReadBytesAsync(compressedFiles[i].Path.LocalPath);
                    raw.Parse(fileData);
                    FileItems.Add(new FileItem
                    {
                        Name = Path.GetFileName(compressedFiles[i].Path.LocalPath),
                        Path = compressedFiles[i].Path.LocalPath
                    });
                }
                catch (InvalidDataException)
                {
                    continue;
                }
                catch (IndexOutOfRangeException)
                {
                    continue;
                }
                
                Progress = (i + 1) / (double)compressedFiles.Count * 100; //Showcase the progress of reading all of the files in memory
            }

            if (FileItems.Count == 1)
                FileIOStatus = FileItems.Count + " file loaded.";
            else if (FileItems.Count == 0)
                FileIOStatus = "No valid file was selected.";
            else
                FileIOStatus = FileItems.Count + " files loaded.";

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
    private async Task LoadPreviewImage(string FilePath)
    {
        byte[] fileData = await _fileIO.ReadBytesAsync(FilePath);
        raw.Parse(fileData);
        using (var stream = new MemoryStream(raw.Export()))
        {
            PreviewImage = new Bitmap(stream);
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