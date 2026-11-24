using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Turune.Models.Formats.Initialization;
using Turune.Services;

namespace Turune.ViewModels.Tools.Initialization;
public partial class INIImportViewModel : ObservableObject
{
    private readonly IStorageService _storageService;
    private readonly IFileIO _fileIO;
    private readonly NavigationViewModel _navigation;
    private readonly IEncodingService _encodings;

    public INIImportViewModel(IStorageService storageService, IFileIO fileIO, NavigationViewModel navigation, IEncodingService encodings)
    {
        _storageService = storageService;
        _fileIO = fileIO;
        _navigation = navigation;
        _encodings = encodings;

        AvailableEncodings = new ObservableCollection<EncodingInformation>(_encodings.AvailableEncodings);
        InputEncoding = AvailableEncodings.Count > 0
            ? AvailableEncodings[0]
            : throw new InvalidOperationException("No encodings available for the given environment.");
    }
    private INI ini = new();

    //Properties screen
    [ObservableProperty] public partial string? FileIOStatus { get; set; } = "Please select a file.";
    [ObservableProperty] public partial bool IsReady { get; set; } = false;
    [ObservableProperty] public partial bool IsBusy { get; set; } = false;
    [ObservableProperty] public partial EncodingInformation InputEncoding { get; set; }
    partial void OnInputEncodingChanged(EncodingInformation value)
    {
        ini = new();
        FileIOStatus = "Please select a file.";
        IsReady = false;
    }
    public ObservableCollection<EncodingInformation> AvailableEncodings { get; }

    [RelayCommand] private void GoBack() => _navigation.GoBack();

    [RelayCommand]
    private async Task SelectInputFile()
    {
        try
        {
            var inputPath = await _storageService.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select file to open",
                FileTypeFilter = [Models.FileType.Initialization.INI],
                SuggestedFileType = Models.FileType.Initialization.INI
            });
            if (inputPath.Count == 0)
            {
                if (IsReady == false)
                    FileIOStatus = "No file was selected.";
                return;
            }
            
            IsBusy = true;
            IsReady = false;
            
            ini = new INI();
            string[] inputData = await _fileIO.ReadLinesAsync(inputPath[0].Path.LocalPath, InputEncoding.Encoding);
            ini.ParseUnpacked(inputData);

            FileIOStatus = inputPath[0].Name + " parsed correctly.";
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
    private async Task Import()
    {
        var saveFile = await _storageService.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save file",
            DefaultExtension = "ini",
            FileTypeChoices = [Models.FileType.Initialization.INI_Compiled]
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
            processingVM.StatusText = "Writing to disk...";
            processingVM.CancellationToken.ThrowIfCancellationRequested();

            byte[] exportedFile = ini.Generate(InputEncoding.Encoding);
            await _fileIO.WriteBytesAsync(saveFile.Path.LocalPath, exportedFile, processingVM.CancellationToken);
            
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