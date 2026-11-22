using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Turune.Models;
using Turune.Services;

namespace Turune.ViewModels.Tools.Scripts;
public partial class SDTDecompileViewModel : ObservableObject
{
    private readonly IStorageService _storageService;
    private readonly IFileIO _fileIO;
    private readonly NavigationViewModel _navigation;
    private readonly IEncodingService _encodings;

    public SDTDecompileViewModel(IStorageService storageService, IFileIO fileIO, NavigationViewModel navigation, IEncodingService encodings)
    {
        _storageService = storageService;
        _fileIO = fileIO;
        _navigation = navigation;
        _encodings = encodings;

        AvailableEncodings = new ObservableCollection<EncodingInfo>(_encodings.AvailableEncodings);
        InputEncoding = AvailableEncodings.Count > 0
            ? AvailableEncodings[0]
            : throw new InvalidOperationException("No encodings available for the given environment.");
        ExportEncoding = InputEncoding;
    }

    //SDT elements
    SDT sdt = new();
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
    private bool HasFileItems() => FileItems.Count > 0;
    [ObservableProperty] public partial EncodingInfo InputEncoding { get; set; }
    partial void OnInputEncodingChanged(EncodingInfo value)
    {
        FileItems.Clear();
        FileIOStatus = "Please select a file.";
        IsReady = false;
    }
    [ObservableProperty] public partial EncodingInfo ExportEncoding { get; set; }

    // Instance-level, sourced from IEncodingService at construction time.
    public ObservableCollection<EncodingInfo> AvailableEncodings { get; }

    [RelayCommand] private void GoBack() => _navigation.GoBack();

    [RelayCommand]
    private async Task SelectInput()
    {
        try
        {
            var inputFiles = await _storageService.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select a file to open",
                AllowMultiple = true,
                FileTypeFilter = [FileType.Script.SDT]
            });
            if (inputFiles.Count == 0)
            {
                //Only set the sentence of no file selected in case the user already selected previously accepted files.
                if (IsReady == false)
                    FileIOStatus = "No file was selected.";
                return;
            }
            
            Progress = 0;
            IsBusy = true;
            IsReady = false;

            FileItems.Clear();
            for (int i = 0; i < inputFiles.Count; i++)
            {
                try
                {
                    byte[] fileData = await _fileIO.ReadBytesAsync(inputFiles[i].Path.LocalPath);
                    sdt.ParseSDT(fileData, InputEncoding.Encoding);
                    FileItems.Add(new FileItem
                    {
                        Name = Path.GetFileNameWithoutExtension(inputFiles[i].Path.LocalPath),
                        Path = inputFiles[i].Path.LocalPath
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
                
                Progress = (i + 1) / (double)inputFiles.Count * 100; //Showcase the progress of reading all of the files in memory
            }

            if (FileItems.Count == 1)
                FileIOStatus = FileItems.Count + " file loaded.";
            else if (FileItems.Count == 0)
            {
                FileIOStatus = "No valid file was selected.";
                IsReady = false;
                return;
            }
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
            DecompileCommand.NotifyCanExecuteChanged();
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(HasFileItems))]
    private async Task Decompile()
    {
        var saveFolder = await _storageService.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select the output folder"
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
                if (FileItems.Count == 1)
                    processingVM.StatusText = "Writing file to disk...";
                else
                    processingVM.StatusText = "Writing file " + i + " of " + FileItems.Count + " to disk...";
                string saveFilePath = Path.Combine(saveFolder[0].Path.LocalPath, FileItems[i].Name + ".cpp");

                byte[] inputFile = await _fileIO.ReadBytesAsync(FileItems[i].Path);
                sdt.ParseSDT(inputFile, ExportEncoding.Encoding);
                await _fileIO.WriteLinesAsync(saveFilePath, sdt.Export(), ExportEncoding.Encoding, processingVM.CancellationToken);
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