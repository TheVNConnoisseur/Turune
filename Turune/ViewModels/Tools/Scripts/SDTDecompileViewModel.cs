using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
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

    public SDTDecompileViewModel(IStorageService storageService, IFileIO fileIO, NavigationViewModel navigation)
    {
        _storageService = storageService;
        _fileIO = fileIO;
        _navigation = navigation;
    }

    //SDT elements
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
    [ObservableProperty] public partial EncodingOption SelectedEncoding { get; set; } = AvailableEncodings[0];
    public record EncodingOption(string DisplayName, Encoding Encoding)
    {
        public override string ToString() => DisplayName;
    }
    public static ObservableCollection<EncodingOption> AvailableEncodings { get; } =
    [
        new EncodingOption("Shift-JIS", Encoding.GetEncoding("shift_jis")),
        new EncodingOption("UTF-8", Encoding.UTF8),
        new EncodingOption("ASCII", Encoding.ASCII),
        new EncodingOption("UTF-16 LE", Encoding.Unicode)
    ];

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
                    SDT sdt = new();
                    sdt.ParseSDT(fileData, SelectedEncoding.Encoding);
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
            DecompileAllCommand.NotifyCanExecuteChanged();
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Decompile()
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
            // await _fileIO.WriteBytesAsync(saveFile.Path.LocalPath, sdt.Decompile(), processingVM.CancellationToken);
            
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
    private async Task DecompileAll()
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
                // sdt.ParseBitmap(inputFile);
                // await _fileIO.WriteBytesAsync(saveFilePath, sdt.Decompile(), processingVM.CancellationToken);
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