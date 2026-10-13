using System;
using System.Collections.Generic;
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

    //ODN properties
    [ObservableProperty] public partial bool IsEncrypted { get; set; }

    [ObservableProperty] public partial bool EntriesOffsetsAreRelativeToRawData { get; set; }

    [ObservableProperty] public partial bool HasDelimiterEntryOffset { get; set; }

    //ODN elements
    private ODN odn = new();
    public byte[][] filesData = [];
    public string[] filesNames = [];

    //Properties screen
    [ObservableProperty] public partial string? ODNParsingStatus { get; set; } = "Please select a folder.";
    [ObservableProperty] public partial bool IsODNCreationReady { get; set; } = false;
    [ObservableProperty] public partial bool IsBusy { get; set; } = false;
    [ObservableProperty] public partial double Progress { get; set; } = 0;

    [RelayCommand] private void GoBack() => _navigation.GoBack();

    [RelayCommand]
    private async Task SelectInputFolder()
    {
        try
        {
            var uncompressedFilesFolder = await _storageService.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select folder with the files to compress"
            });
            if (uncompressedFilesFolder.Count == 0)
            {
                //Only set the sentence of no folder selected in case the user already selected previously accepted files.
                if (IsODNCreationReady == false)
                    ODNParsingStatus = "No folder was selected.";
                return;
            }
            
            IsBusy = true;
            Progress = 0;
            IsODNCreationReady = false;
            
            odn = new ODN();

            //Because of an issue with Linux's IO handling, the way of obtaining the file names is through an FolderPicker
            List<string> uncompressedFilesPaths = Directory.GetFiles(uncompressedFilesFolder[0].Path.LocalPath).ToList();
            uncompressedFilesPaths.Sort();  //Ensure that all files are ordered alphabetically, because otherwise the list is ordered at random
        
            filesData = new byte[uncompressedFilesPaths.Count][];
            filesNames = new string[uncompressedFilesPaths.Count];

            for (int i = 0; i < uncompressedFilesPaths.Count; i++)
            {
                filesData[i] = await _fileIO.ReadBytesAsync(uncompressedFilesPaths[i]);
                filesNames[i] = Path.GetFileName(uncompressedFilesPaths[i]);

                Progress = (i + 1) / (double)uncompressedFilesPaths.Count * 100; //Showcase the progress of reading all of the files in memory
            }

            odn.ParseUnpacked(filesNames, filesData);
            ODNParsingStatus = filesNames.Length + " files parsed correctly.";
            IsODNCreationReady = true;
        }
        catch (Exception ex)
        {
            ODNParsingStatus = ex.Message;
            IsODNCreationReady = false;
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
        var odnFile = await _storageService.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save ODN file",
            DefaultExtension = "odn",
            FileTypeChoices = [Models.FileType.Container_ODN]
        });
        if (odnFile is null) return;

        //Pass the entire process onto an external VM
        var processingVM = new ProcessingViewModel(_navigation)
        {
            Title = "Creating ODN file...",
            IsIndeterminate = true
        };

        _navigation.NavigateTo(processingVM);

        try
        {
            processingVM.StatusText = "Building file structure...";
            processingVM.CancellationToken.ThrowIfCancellationRequested();
            byte[] createdODN = odn.Create(IsEncrypted, EntriesOffsetsAreRelativeToRawData, HasDelimiterEntryOffset);

            processingVM.StatusText = "Writing to disk...";
            await _fileIO.WriteBytesAsync(odnFile.Path.LocalPath, createdODN, processingVM.CancellationToken);
            
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