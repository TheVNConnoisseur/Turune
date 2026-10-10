using System.IO;
using System.Threading.Tasks;
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

    [RelayCommand]
    private void GoBack() => _navigation.GoBack();

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