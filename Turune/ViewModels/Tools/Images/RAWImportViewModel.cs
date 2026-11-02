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
using Turune.Models;
using Turune.Models.Formats.Images;
using Turune.Services;

namespace Turune.ViewModels.Tools.Images;
public partial class RAWImportViewModel(
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
    public partial class FileItem : ObservableObject
    {
        public required string Name { get; set; } = string.Empty;
        public required string Path { get; set; } = string.Empty;
        [ObservableProperty] public partial bool IsSelected { get; set; } = false;
    }

    //Properties screen
    [ObservableProperty] public partial string? FileIOStatus { get; set; } = "Please select a file.";
    [ObservableProperty] public partial bool IsReady { get; set; } = false;
    [ObservableProperty] public partial bool IsBusy { get; set; } = false;
    [ObservableProperty] public partial double Progress { get; set; } = 0;
    [ObservableProperty] public partial Bitmap? PreviewImage { get; set; }  //Bitmap that holds the previewed image
    private bool HasFileItems() => FileItems.Count > 0; //Check for the "Import all images" button

    [RelayCommand] private void GoBack() => _navigation.GoBack();

    [RelayCommand]
    private async Task SelectInputFile()
    {
        try
        {
            var compressedFiles = await _storageService.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select an image to open",
                AllowMultiple = true,
                FileTypeFilter = [FileType.Common.BMP]
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
                    raw.ParseBitmap(fileData);
                    FileItems.Add(new FileItem
                    {
                        Name = Path.GetFileNameWithoutExtension(compressedFiles[i].Path.LocalPath),
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
        }
        finally
        {
            ImportAllCommand.NotifyCanExecuteChanged();
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadPreviewImage(string FilePath)
    {
        foreach (var item in FileItems)
        {
            if (item.Path == FilePath)
                item.IsSelected = true;
            else
                item.IsSelected = false;
        }

        byte[] fileData = await _fileIO.ReadBytesAsync(FilePath);
        using var stream = new MemoryStream(fileData);
        PreviewImage = new Bitmap(stream);
    }

    [RelayCommand]
    private async Task Import()
    {
        var saveFile = await _storageService.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save file"
        });
        if (saveFile is null) return;

        //Pass the entire process onto an external VM
        var processingVM = new ProcessingViewModel(_navigation)
        {
            Title = "Converting file...",
            IsIndeterminate = true
        };

        _navigation.NavigateTo(processingVM);

        try
        {
            processingVM.CancellationToken.ThrowIfCancellationRequested();
            processingVM.StatusText = "Writing file to disk...";
            await _fileIO.WriteBytesAsync(saveFile.Path.LocalPath, raw.Import(), processingVM.CancellationToken);
            
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

    [RelayCommand(CanExecute = nameof(HasFileItems))]
    private async Task ImportAll()
    {
        var saveFolder = await _storageService.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select the folder to save the files to"
        });
        if (saveFolder.Count == 0) return;

        //Pass the entire process onto an external VM
        var processingVM = new ProcessingViewModel(_navigation)
        {
            Title = "Converting files...",
            IsIndeterminate = false
        };

        _navigation.NavigateTo(processingVM);

        try
        {
            processingVM.CancellationToken.ThrowIfCancellationRequested();
            
            //Write all of the loaded files to the selected path
            for (int i = 0; i < FileItems.Count; i++)
            {
                processingVM.StatusText = "Writing file " + i + " of " + FileItems.Count + " to disk...";
                string saveFilePath = Path.Combine(saveFolder[0].Path.LocalPath, FileItems[i].Name);

                //The images are not loaded in memory, they have to be read and parsed before converting them
                byte[] inputFile = await _fileIO.ReadBytesAsync(FileItems[i].Path);
                raw.ParseBitmap(inputFile);
                await _fileIO.WriteBytesAsync(saveFilePath, raw.Import(), processingVM.CancellationToken);
                processingVM.Progress = (i + 1) / (double)FileItems.Count * 100;
            }
            
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