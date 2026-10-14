using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Turune.Models.Formats.Containers;
using Turune.Services;

namespace Turune.ViewModels.Tools.Containers;
public partial class ODNExtractViewModel(
    IStorageService storageService,
    IFileIO fileIO,
    NavigationViewModel navigation) : ObservableObject
{
    private readonly IStorageService _storageService = storageService;
    private readonly IFileIO _fileIO = fileIO;
    private readonly NavigationViewModel _navigation = navigation;

    //ODN properties
    [ObservableProperty] public partial string IsEncrypted { get; set; } = "-";
    [ObservableProperty] public partial string EntriesOffsetsAreRelativeToRawData { get; set; } = "-";
    [ObservableProperty] public partial string HasDelimiterEntry { get; set; } = "-";

    //ODN elements
    private ODN odn = new();
    public byte[] fileData = [];

    //Properties screen
    [ObservableProperty] public partial string? ODNParsingStatus { get; set; } = "Please select a file.";
    [ObservableProperty] public partial bool IsODNExtractionReady { get; set; } = false;
    [ObservableProperty] public partial bool IsBusy { get; set; } = false;

    [RelayCommand] private void GoBack() => _navigation.GoBack();

    [RelayCommand]
    private async Task SelectInputFile()
    {
        try
        {
            var compressedFile = await _storageService.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select ODN file to open",
                FileTypeFilter = [Models.FileType.Container_ODN],
                SuggestedFileType = Models.FileType.Container_ODN
            });
            if (compressedFile.Count == 0)
            {
                //Only set the sentence of no file selected in case the user already selected previously an accepted file.
                if (IsODNExtractionReady == false)
                {
                    ODNParsingStatus = "No file was selected.";
                    IsEncrypted = "-";
                    EntriesOffsetsAreRelativeToRawData = "-";
                    HasDelimiterEntry = "-";
                }
                return;
            }
            
            IsBusy = true;
            IsODNExtractionReady = false;
            
            odn = new ODN();
            fileData = await _fileIO.ReadBytesAsync(compressedFile[0].Path.LocalPath);
            odn.ParsePacked(fileData);
            
            //Parse all of the information from the ODN file
            if (odn.OdnScheme.Properties.IsEncrypted)
                IsEncrypted = Localizations.Resources.CommonTerms_Yes;
            else
                IsEncrypted = Localizations.Resources.CommonTerms_No;
            
            if (odn.OdnScheme.Properties.EntriesOffsetsAreRelativeToRawData)
                EntriesOffsetsAreRelativeToRawData = Localizations.Resources.CommonTerms_Yes;
            else
                EntriesOffsetsAreRelativeToRawData = Localizations.Resources.CommonTerms_No;
            
            if (odn.OdnScheme.Properties.HasDelimiterEntry)
                HasDelimiterEntry = Localizations.Resources.CommonTerms_Yes;
            else
                HasDelimiterEntry = Localizations.Resources.CommonTerms_No;

            ODNParsingStatus = compressedFile[0].Name + " parsed correctly.";
            IsODNExtractionReady = true;
        }
        catch (Exception ex)
        {
            ODNParsingStatus = ex.Message;
            IsODNExtractionReady = false;
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
        var files = await _storageService.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open ODN",
            AllowMultiple = true
        });
        if (files.Count == 0) return;

        var saveFolder = await _storageService.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Save Exported Files"
        });
        if (saveFolder.Count == 0) return;

        foreach (var file in files)
        {
            var bytes = await _fileIO.ReadBytesAsync(file.Path.LocalPath);
            var odn = new ODN();
            odn.ParsePacked(bytes);

            foreach (var entry in odn.OdnScheme.Entries)
            {
                var path = Path.Combine(saveFolder[0].Path.LocalPath, entry.Name);
                await _fileIO.WriteBytesAsync(path, entry.Data);
            }
        }
    }

    [RelayCommand]
    private void Back() => _navigation.GoBack();
}