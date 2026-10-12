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
    [ObservableProperty] private bool _isEncrypted;
    [ObservableProperty] private bool _entriesOffsetsAreRelativeToRawData;
    [ObservableProperty] private bool _hasDelimiterEntryOffset;

    [RelayCommand] private void GoBack() => _navigation.GoBack();

    [RelayCommand]
    private async Task Create()
    {
        var uncompressedFilesFolder = await _storageService.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select folder with the files to compress"
        });
        if (uncompressedFilesFolder is null) return;

        //Because of an issue with Linux's IO handling, the way of obtaining the file names is through an FolderPicker
        List<string> uncompressedFilesPaths = Directory.GetFiles(uncompressedFilesFolder[0].Path.LocalPath).ToList();
        uncompressedFilesPaths.Sort();  //Ensure that all files are ordered alphabetically, because otherwise the list is ordered at random

        byte[][] filesData = new byte[uncompressedFilesPaths.Count][];
        string[] filesNames = new string[uncompressedFilesPaths.Count];

        for (int i = 0; i < uncompressedFilesPaths.Count; i++)
        {
            filesData[i] = await _fileIO.ReadBytesAsync(uncompressedFilesPaths[i]);
            filesNames[i] = Path.GetFileName(uncompressedFilesPaths[i]);
        }

        var odn = new ODN();
        odn.ParseUnpacked(filesNames, filesData);

        var saveFile = await _storageService.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save ODN file"
        });
        if (saveFile is null) return;

        byte[] createdODN = odn.Create(IsEncrypted, EntriesOffsetsAreRelativeToRawData, HasDelimiterEntryOffset);
        await _fileIO.WriteBytesAsync(saveFile.Path.LocalPath, createdODN);
    }
}