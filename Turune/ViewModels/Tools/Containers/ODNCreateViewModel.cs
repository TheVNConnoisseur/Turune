using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Turune.Models.Formats.Containers;
using Turune.Services;

namespace Turune.ViewModels.Tools.Containers;
public partial class ODNCreateViewModel(
    IStorageService storageService,
    IFileIO fileIO,
    NavigationViewModel navigation) : ObservableObject
{
    private readonly IStorageService _storageService = storageService;
    private readonly IFileIO _fileIO = fileIO;
    private readonly NavigationViewModel _navigation = navigation;
    private ODN odn = new();
    public ObservableCollection<FileItem> FileItems { get; } = [];
    public partial class FileItem : ObservableObject
    {
        public required string Name { get; set; } = string.Empty;
        public required byte[] Data { get; set; } = [];
    }

    //ODN properties
    [ObservableProperty] public partial bool IsEncrypted { get; set; }
    [ObservableProperty] public partial bool EntriesOffsetsAreRelativeToRawData { get; set; }
    [ObservableProperty] public partial bool HasDelimiterEntry { get; set; }

    //Properties screen
    [ObservableProperty] public partial string? FileIOStatus { get; set; } = "Please select a folder.";
    [ObservableProperty] public partial bool IsReady { get; set; } = false;
    [ObservableProperty] public partial bool IsBusy { get; set; } = false;
    [ObservableProperty] public partial double Progress { get; set; } = 0;
    private bool HasFileItems() => FileItems.Count > 0;

    [RelayCommand] private void GoBack() => _navigation.GoBack();

    [RelayCommand]
    private async Task SelectInput()
    {
        try
        {
            // var inputPath = await _storageService.OpenFolderPickerAsync(new FolderPickerOpenOptions
            // {
            //     Title = "Select input folder"
            // });
            var inputPath = await _storageService.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select a compressed image to open",
                AllowMultiple = true
            });
            if (inputPath.Count == 0)
            {
                //Only set the sentence of no folder selected in case the user already selected previously accepted files.
                if (IsReady == false)
                    FileIOStatus = "No folder was selected.";
                return;
            }
            
            IsBusy = true;
            Progress = 0;
            IsReady = false;
            
            FileItems.Clear();
            odn = new ODN();
            inputPath = inputPath.OrderBy(p => Path.GetFileName(p.Path.LocalPath)).ToList();    //Ensure that it is ordered alphabetically

            for (int i = 0; i < inputPath.Count; i++)
            {
                FileItems.Add(new FileItem
                    {
                        Name = Path.GetFileName(inputPath[i].Path.LocalPath),
                        Data = await _fileIO.ReadBytesAsync(inputPath[i].Path.LocalPath)
                    });

                Progress = (i + 1) / (double)inputPath.Count * 100; //Showcase the progress of reading all of the files in memory
            }

            
            var fileItems_Names = FileItems.Select(f => f.Name).ToArray();
            var fileItems_Data = FileItems.Select(f => f.Data).ToArray();
            odn.ParseUnpacked(fileItems_Names, fileItems_Data);
            FileIOStatus = FileItems.Count + " files parsed correctly.";
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
    private async Task Create()
    {
        var saveFile = await _storageService.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save file",
            DefaultExtension = "odn",
            FileTypeChoices = [Models.FileType.Container.ODN]
        });
        if (saveFile is null) return;

        //Pass the entire process onto an external VM
        var processingVM = new ProcessingViewModel(_navigation)
        {
            Title = "Processing...",
            IsIndeterminate = true
        };

        _navigation.NavigateTo(processingVM);

        try
        {
            processingVM.StatusText = "Writing to disk...";
            processingVM.CancellationToken.ThrowIfCancellationRequested();

            byte[] exportFile = odn.Create(IsEncrypted, EntriesOffsetsAreRelativeToRawData, HasDelimiterEntry);
            await _fileIO.WriteBytesAsync(saveFile.Path.LocalPath, exportFile, processingVM.CancellationToken);
            
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