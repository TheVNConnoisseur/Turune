using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Turune.Services;

namespace Turune.ViewModels.Tools.Initialization;
public partial class DATExportViewModel : ObservableObject
{
    private readonly IStorageService _storageService;
    private readonly IFileIO _fileIO;
    private readonly NavigationViewModel _navigation;
    private readonly IEncodingService _encodings;

    public DATExportViewModel(IStorageService storageService, IFileIO fileIO, NavigationViewModel navigation, IEncodingService encodings)
    {
        _storageService = storageService;
        _fileIO = fileIO;
        _navigation = navigation;
        _encodings = encodings;

        AvailableEncodings = new ObservableCollection<EncodingInformation>(_encodings.AvailableEncodings);
        ExportEncoding = AvailableEncodings.Count > 0
            ? AvailableEncodings[0]
            : throw new InvalidOperationException("No encodings available for the given environment.");
    }

    //DAT elements
    private DAT dat = new();

    //Properties screen
    [ObservableProperty] public partial string? FileIOStatus { get; set; } = "Please select a file.";
    [ObservableProperty] public partial bool IsReady { get; set; } = false;
    [ObservableProperty] public partial bool IsBusy { get; set; } = false;
    [ObservableProperty] public partial EncodingInformation InputEncoding { get; set; }
    partial void OnInputEncodingChanged(EncodingInformation value)
    {
        dat = new();
        FileIOStatus = "Please select a file.";
        IsReady = false;
    }
    [ObservableProperty] public partial EncodingInformation ExportEncoding { get; set; }
    public ObservableCollection<EncodingInformation> AvailableEncodings { get; }

    [RelayCommand] private void GoBack() => _navigation.GoBack();

    [RelayCommand]
    private async Task SelectInput()
    {
        try
        {
            var inputPath = await _storageService.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select file to open",
                FileTypeFilter = [Models.FileType.Initialization.DAT],
                SuggestedFileType = Models.FileType.Initialization.DAT
            });
            if (inputPath.Count == 0)
            {
                //Only set the sentence of no file selected in case the user already selected previously an accepted file.
                if (IsReady == false)
                    FileIOStatus = "No file was selected.";
                return;
            }
            
            IsBusy = true;
            IsReady = false;
            
            dat = new DAT();
            byte[] inputFile = await _fileIO.ReadBytesAsync(inputPath[0].Path.LocalPath);
            dat.ParsePacked(inputFile, InputEncoding.Encoding);

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
    private async Task Export()
    {
        var exportPath = await _storageService.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save file",
            DefaultExtension = "ini",
            FileTypeChoices = [Models.FileType.Initialization.INI]
        });
        if (exportPath is null) return;

        //Pass the entire process onto an external VM
        var processingVM = new ProcessingViewModel(_navigation)
        {
            Title = "Processing...",
            IsIndeterminate = true
        };

        _navigation.NavigateTo(processingVM);

        try
        {
            processingVM.CancellationToken.ThrowIfCancellationRequested();
            processingVM.StatusText = "Writing to disk...";

            string[] exportFile = dat.Export();
            await _fileIO.WriteLinesAsync(exportPath.Path.LocalPath, exportFile, ExportEncoding.Encoding, processingVM.CancellationToken);
            
            processingVM.Progress = 100;
            processingVM.IsIndeterminate = false;
            processingVM.StatusText = "Done.";
            processingVM.State = ProcessingState.Succeeded;
        }
        catch (EncoderFallbackException)
        {
            processingVM.StatusText =
                $"Cannot export to {ExportEncoding.DisplayName}: try using an Unicode encoding or the original encoding instead.";
            processingVM.State = ProcessingState.Failed;
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