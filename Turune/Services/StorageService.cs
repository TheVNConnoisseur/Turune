using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Turune.Services;

// Methods from https://docs.avaloniaui.net/docs/services/storage/storage-provider#methods
public interface IStorageService
{
    Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options);
    Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options);
    Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options);
}

public sealed class StorageService(Func<IStorageProvider?> getStorageProvider) : IStorageService
{
    private IStorageProvider GetStorageProvider() =>
        getStorageProvider() ?? throw new InvalidOperationException("No active storage provider is available.");

    public Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options) =>
        GetStorageProvider().OpenFilePickerAsync(options);

    public Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options) =>
        GetStorageProvider().SaveFilePickerAsync(options);

    public Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options) =>
        GetStorageProvider().OpenFolderPickerAsync(options);
}